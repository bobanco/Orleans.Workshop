using Azro.Core.Services.Api.Players;
using Azro.Core.Services.Tests.Orleans;
using System;
using System.Threading.Tasks;
using Azro.Core.Services.Api.Utils;
using Orleans;
using Orleans.Runtime;
using Orleans.Storage;
using Xunit;
using Xunit.Abstractions;

namespace Azro.Core.Services.Tests
{
    public class PlayerTests : HostedTestClusterEnsureDefaultStarted
    {
        public static readonly PlayerProfile DefaultPlayerProfile = new PlayerProfile
        {
            BirthDate = DateTime.Parse("2018-12-08 22:20:00"),
            CreatedAt = DateTimeOffset.UtcNow,
            Email = "game-master@azro.com",
            Mobile = "123456789",
            Name = "Bob",
            NickName = "X-99"
        };

        private readonly ITestOutputHelper _output;

        public PlayerTests(ITestOutputHelper output, DefaultClusterFixture fixture) 
            : base(fixture)
        {
            _output = output;
        }

        [Fact]
        public async Task AnyOperationShouldThrowExceptionIfPlayerIsNotCreated()
        {
            var player = GrainFactory.GetGrain<IPlayer>(Guid.NewGuid().ToString());
            
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => await player.Play());
            Assert.NotNull(exception);
            Assert.NotNull(exception.Message);
            Assert.Equal("Player doesn't exists.", exception.Message);

            exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => await player.JoinGame(null));
            Assert.NotNull(exception);
            Assert.NotNull(exception.Message);
            Assert.Equal("Player doesn't exists.", exception.Message);

            exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => await player.GetStatus());
            Assert.NotNull(exception);
            Assert.NotNull(exception.Message);
            Assert.Equal("Player doesn't exists.", exception.Message);
        }

        [Fact]
        public async Task CreatePlayerTest()
        {
            var player = GrainFactory.GetGrain<IPlayer>(Guid.NewGuid().ToString());
            await player.Create(DefaultPlayerProfile);
            var playerInfo = await player.GetInfo();
            Assert.NotNull(playerInfo);
            Assert.Equal(PlayerStatus.Offline, playerInfo.Status);
            Assert.Null(playerInfo.CurrentGame);
            Assert.NotNull(playerInfo.Profile);
            Assert.Equal("Bob", playerInfo.Profile.Name);
        }

        [Fact]
        public async Task LoginPlayerTest()
        {
            var player = GrainFactory.GetGrain<IPlayer>(Guid.NewGuid().ToString());
            await player.Create(DefaultPlayerProfile);
            var status = await player.GetStatus();
            Assert.Equal(PlayerStatus.Offline, status);
            await player.Login();
            status = await player.GetStatus();
            Assert.Equal(PlayerStatus.Online, status);
        }

        [Fact]
        public async Task PlayerShouldNotBeAbleToPlayIfItsNotInGame()
        {
            var player = GrainFactory.GetGrain<IPlayer>(Guid.NewGuid().ToString());
            await player.Create(DefaultPlayerProfile);
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => await player.Play());
            Assert.NotNull(exception);
            Assert.NotNull(exception.Message);
            Assert.Equal("Player is not currently in game.", exception.Message);
        }

        [Fact]
        public async Task FinishGameShouldThrowExceptionIfInvalidGameIsProvided()
        {
            var player = GrainFactory.GetGrain<IPlayer>(Guid.NewGuid().ToString());
            await player.Create(DefaultPlayerProfile);
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () => await player.FinishGame(null));
            Assert.NotNull(exception);
            Assert.NotNull(exception.Message);
        }

        [Fact]
        public async Task JoinGameShouldThrowExceptionIfInvalidGameIsProvided()
        {
            var player = GrainFactory.GetGrain<IPlayer>(Guid.NewGuid().ToString());
            await player.Create(DefaultPlayerProfile);
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () => await player.JoinGame(null));
            Assert.NotNull(exception);
            Assert.NotNull(exception.Message);
        }

    }
}
