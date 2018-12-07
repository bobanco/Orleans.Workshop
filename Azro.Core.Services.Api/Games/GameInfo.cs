using Azro.Core.Services.Api.Players;
using System;
using System.Collections.Generic;

namespace Azro.Core.Services.Api.Games
{
    [Serializable]
    public class GameInfo
    {
        public IReadOnlyList<IPlayer> Players { get; set; }
        public IReadOnlyDictionary<IPlayer, PlayerScore> Scores { get; set; }
        public GameStatus Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
