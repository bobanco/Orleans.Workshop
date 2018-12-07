using System;
using System.Collections.Generic;
using Azro.Core.Services.Api.Games;
using Azro.Core.Services.Api.Players;

namespace Azro.Core.Services.Impl
{
    [Serializable]
    public sealed class PlayerState
    {
        public bool IsCreated { get; set; }
        public PlayerStatus Status { get; set; }
        public PlayerProfile Profile { get; set; }
        public  List<IGame> GamesPlayed { get; set; }
        public  List<IPlayer> Friends { get; set; }
        public IGame CurrentGame { get; set; }
    }
}
