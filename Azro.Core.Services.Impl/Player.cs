using Azro.Core.Services.Api.Games;
using Azro.Core.Services.Api.Players;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Azro.Core.Services.Impl
{
    public class Player : Grain<PlayerState>, IPlayer
    {
        public async Task Create(PlayerProfile playerInfo)
        {
            if(State.IsCreated)
                throw new InvalidOperationException("Player already exists.");
            State.Status = PlayerStatus.Offline;
            State.IsCreated = true;
            State.Friends = new List<IPlayer>();
            State.GamesPlayed = new List<IGame>();
            State.Profile = playerInfo;
            await WriteStateAsync();
        }

        public async Task UpdateInfo(PlayerProfile playerInfo)
        {
            if (!State.IsCreated)
                throw new InvalidOperationException("Player doesn't exists.");
            State.Profile = playerInfo ?? throw new ArgumentNullException(nameof(playerInfo));
            await WriteStateAsync();
        }

        public async Task Login()
        {
            if (!State.IsCreated)
                throw new InvalidOperationException("Player doesn't exists.");
            State.Status = PlayerStatus.Online;
            await WriteStateAsync();
        }

        public  async Task Logout()
        {
            if (!State.IsCreated)
                throw new InvalidOperationException("Player doesn't exists.");
            State.Status = PlayerStatus.Offline;
            await WriteStateAsync();
        }

        public async Task Play()
        {
            if (!State.IsCreated)
                throw new InvalidOperationException("Player doesn't exists.");
            if(State.CurrentGame==null)
                throw new InvalidOperationException("Player is not currently in game.");
            State.Status = PlayerStatus.Playing;
            await WriteStateAsync();
        }

        public async Task Invisible()
        {
            if (!State.IsCreated)
                throw new InvalidOperationException("Player doesn't exists.");
            State.Status = PlayerStatus.Invisible;
            await WriteStateAsync();
        }

        public async Task JoinGame(IGame game)
        {
            if (!State.IsCreated)
                throw new InvalidOperationException("Player doesn't exists.");
            if (game == null)
                throw new ArgumentNullException(nameof(game));
            State.Status = PlayerStatus.WaitingForGameToStart;
            State.CurrentGame = game;
            await WriteStateAsync();
        }

        public async Task FinishGame(IGame game)
        {
            if (!State.IsCreated)
                throw new InvalidOperationException("Player doesn't exists.");
            if(game==null)
                throw new ArgumentNullException(nameof(game));
            State.GamesPlayed.Add(game);
            State.Status = PlayerStatus.Online;
            await WriteStateAsync();
        }

        public  async Task LeaveGame(IGame game)
        {
            if (!State.IsCreated)
                throw new InvalidOperationException("Player doesn't exists.");
            if (game == null)
                throw new ArgumentNullException(nameof(game));
            State.Status = PlayerStatus.Online;
            State.CurrentGame = null;
            await WriteStateAsync();
        }

        public Task<PlayerStatus> GetStatus()
        {
            if (!State.IsCreated)
                throw new InvalidOperationException("Player doesn't exists.");
            return Task.FromResult<PlayerStatus>(State.Status);
        }

        public Task<IGame> GetCurrentGame()
        {
            if (!State.IsCreated)
                throw new InvalidOperationException("Player doesn't exists.");
            return Task.FromResult<IGame>(State.CurrentGame);
        }

        public Task<PlayerInfo> GetInfo()
        {
            if (!State.IsCreated)
                throw new InvalidOperationException("Player doesn't exists.");
            return Task.FromResult<PlayerInfo>(new PlayerInfo
            {
                CurrentGame = State.CurrentGame,
                Status = State.Status,
                Profile = State.Profile
            });
        }

        public Task<IReadOnlyList<IPlayer>> GetFriends()
        {
            if (!State.IsCreated)
                throw new InvalidOperationException("Player doesn't exists.");
            return Task.FromResult<IReadOnlyList<IPlayer>>(State.Friends.ToList());
        }

        public Task<IReadOnlyList<IGame>> GetPlayedGames()
        {
            if (!State.IsCreated)
                throw new InvalidOperationException("Player doesn't exists.");
            return Task.FromResult<IReadOnlyList<IGame>>(State.GamesPlayed.ToList());
        }
    }
}
