using System.Collections.Generic;
using System.Threading.Tasks;
using Azro.Core.Services.Api.Games;
using Orleans;
using Orleans.CodeGeneration;

namespace Azro.Core.Services.Api.Players
{
    [Version(1)]
    public interface IPlayer : IGrainWithStringKey
    {
        Task Create(PlayerProfile playerInfo);
        Task UpdateInfo(PlayerProfile playerInfo);
        Task Login();
        Task Logout();
        Task Play();
        Task Invisible();
        Task JoinGame(IGame game);
        Task FinishGame(IGame game);
        Task LeaveGame(IGame game);
        Task<PlayerStatus> GetStatus();
        Task<IGame> GetCurrentGame();
        Task<PlayerInfo> GetInfo();
        Task<IReadOnlyList<IPlayer>> GetFriends();
        Task<IReadOnlyList<IGame>> GetPlayedGames();
    }
}
