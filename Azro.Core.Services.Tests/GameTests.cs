using Azro.Core.Services.Api.Games;
using Azro.Core.Services.Tests.Orleans;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Azro.Core.Services.Tests
{
    public class GameTests : HostedTestClusterEnsureDefaultStarted
    {
        public const string DefaultGameId = "GAME1";
        private readonly ITestOutputHelper _output;

        public GameTests(ITestOutputHelper output, DefaultClusterFixture fixture) : base(fixture)
        {
            _output = output;
        }

        [Fact]
        public async Task CreateGameTest()
        {
            var gameOptions = new GameOptions {MaxNumberOfPlayers = 1};
            var gameGrain = GrainFactory.GetGrain<IGame>(DefaultGameId);
            await gameGrain.Create(gameOptions);
            var gameInfo = await gameGrain.GetInfo();
            Assert.NotNull(gameInfo);
            Assert.Equal(GameStatus.WaitingForPlayers, gameInfo.Status);
            Assert.Equal(0, gameInfo.Players.Count);
            Assert.Equal(0, gameInfo.Scores.Count);
        }
    }
}
