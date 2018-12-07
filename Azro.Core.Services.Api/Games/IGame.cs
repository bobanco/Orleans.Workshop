using Azro.Core.Services.Api.Players;
using Orleans;
using Orleans.CodeGeneration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Azro.Core.Services.Api.Games
{
    [Version(1)]
    public interface IGame : IGrainWithStringKey
    {
        Task Create(GameOptions gameOptions);
        Task AddPlayer(IPlayer player);
        Task UpdateScore(IPlayer player, PlayerScore score);
        Task Start();
        Task<GameStatus> GetStatus();
        Task Finish();
        Task<GameInfo> GetInfo();
        Task<IReadOnlyList<IPlayer>> GetPlayers();

    }
}
