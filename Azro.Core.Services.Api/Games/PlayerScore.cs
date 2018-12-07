using System;

namespace Azro.Core.Services.Api.Games
{
    [Serializable]
    public class PlayerScore
    {
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Assistants { get; set; }
    }
}
