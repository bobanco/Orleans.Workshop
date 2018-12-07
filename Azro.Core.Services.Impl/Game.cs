using Azro.Core.Services.Api.Games;
using Azro.Core.Services.Api.Players;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Azro.Core.Services.Impl
{
    public class Game : Grain<GameState>, IGame
    {
        public async Task Create(GameOptions gameOptions)
        {
            if(State.IsCreated)
                throw new InvalidOperationException("Game already exists");
            State.IsCreated = true;
            State.CreatedAt = DateTimeOffset.UtcNow;
            State.GameOptions = gameOptions;
            State.Players = new List<IPlayer>();
            State.Scores = new Dictionary<IPlayer, PlayerScore>();
            State.Status = GameStatus.WaitingForPlayers;
            await WriteStateAsync();
        }

        public async Task AddPlayer(IPlayer player)
        {
            if (!State.IsCreated)
                throw new InvalidOperationException("Game doesn't exists");
            if(State.Players.Count + 1 > State.GameOptions.MaxNumberOfPlayers)
                throw new InvalidOperationException("All slots are occupied.");
            State.Players.Add(player);
            State.Scores.Add(player, new PlayerScore());
            await WriteStateAsync();
            await player.JoinGame(this);
        }

        public async Task UpdateScore(IPlayer player, PlayerScore score)
        {
            if (!State.IsCreated)
                throw new InvalidOperationException("Game doesn't exists");
            if (State.Scores.ContainsKey(player))
                State.Scores[player] = score;
            else
            {
                State.Scores.Add(player, score);
            }

            await WriteStateAsync();
        }

        public async Task Start()
        {
            if (!State.IsCreated)
                throw new InvalidOperationException("Game doesn't exists");
            State.StartedAt = DateTimeOffset.UtcNow;
            State.Status = GameStatus.Running; 
            var promises = State.Players.Select(player => player.Play());
            await WriteStateAsync();
            await Task.WhenAll(promises);
        }

        public Task<GameStatus> GetStatus()
        {
            if (!State.IsCreated)
                throw new InvalidOperationException("Game doesn't exists");
            return Task.FromResult<GameStatus>(State.Status);
        }

        public async Task Finish()
        {
            if (!State.IsCreated)
                throw new InvalidOperationException("Game doesn't exists");
            State.CompletedAt = DateTimeOffset.UtcNow;
            State.Status = GameStatus.Finished;
            await WriteStateAsync();
            var promises = new List<Task>(State.Players.Count);
            foreach (var player in State.Players)
            {
                promises.Add(player.JoinGame(this));
            }
            await Task.WhenAll(promises);
        }

        public Task<GameInfo> GetInfo()
        {
            if (!State.IsCreated)
                throw new InvalidOperationException("Game doesn't exists");
            return Task.FromResult<GameInfo>(new GameInfo
            {
                CreatedAt = State.CreatedAt,
                Players = State.Players.ToList(),
                Scores = new Dictionary<IPlayer, PlayerScore>(State.Scores),
                Status = State.Status
            });
        }

        public Task<IReadOnlyList<IPlayer>> GetPlayers()
        {
            if (!State.IsCreated)
                throw new InvalidOperationException("Game doesn't exists");
            return Task.FromResult<IReadOnlyList<IPlayer>>(State.Players.ToList());
        }
    }
}
