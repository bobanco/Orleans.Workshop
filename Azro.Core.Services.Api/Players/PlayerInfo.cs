using System;
using Azro.Core.Services.Api.Games;
using Orleans.Concurrency;

namespace Azro.Core.Services.Api.Players
{
    [Serializable, Immutable]
    public class PlayerInfo
    {
        public PlayerProfile Profile { get; set; }
        public PlayerStatus Status { get; set; }
        public IGame CurrentGame { get; set; }
    }
}
