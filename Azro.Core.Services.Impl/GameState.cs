using System;
using System.Collections.Generic;
using Azro.Core.Services.Api.Games;
using Azro.Core.Services.Api.Players;

namespace Azro.Core.Services.Impl
{
    [Serializable]
    public class GameState
    {
        public bool IsCreated { get; set; }
        public GameOptions GameOptions { get; set; }
        public List<IPlayer> Players { get; set; }
        public Dictionary<IPlayer, PlayerScore> Scores { get; set; }
        public GameStatus Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? StartedAt { get; set; }
        public DateTimeOffset? CompletedAt { get; set; }
    }
}
