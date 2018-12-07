using System;

namespace Azro.Core.Services.Api.Players
{
    [Serializable]
    public class PlayerProfile
    {
        public string Name { get; set; }
        public string NickName { get; set; }
        public byte[] Avatar { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? LastUpdateAt { get; set; }
    }
}
