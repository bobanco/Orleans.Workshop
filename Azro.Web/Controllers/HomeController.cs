using Azro.Core.Services.Api.Games;
using Azro.Core.Services.Api.Players;
using Azro.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azro.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IClusterClient _clusterClient;
        
        public HomeController(IClusterClient clusterClient)
        {
            _clusterClient = clusterClient;
        }
        public async Task<IActionResult> Index()
        {
            var game = _clusterClient.GetGrain<IGame>("1");
            var player = _clusterClient.GetGrain<IPlayer>("1");
            var player2 = _clusterClient.GetGrain<IPlayer>("2");
            await player.Create(new PlayerProfile
            {
                BirthDate = DateTime.Today, CreatedAt = DateTimeOffset.UtcNow,
                Name = "Bob", NickName = "X-99"
            });

            await player2.Create(new PlayerProfile
            {
                BirthDate = DateTime.Today,
                CreatedAt = DateTimeOffset.UtcNow,
                Name = "Marte",
                NickName = "Bambola"
            });

            await game.Create(new GameOptions{ MaxNumberOfPlayers = 2});

            await game.AddPlayer(player);
            await game.AddPlayer(player2);

            await game.Start();

            var status = await game.GetStatus();
            var players = await game.GetPlayers();

            var results = await Task.WhenAll(players.Select(x => x.GetInfo()));

            var playerNicknames = results.Select(x => x.Profile.NickName).ToList();

            await game.UpdateScore(player, new PlayerScore {Kills = 1, Deaths = 1});
            await game.UpdateScore(player2, new PlayerScore {Kills = 1, Deaths = 1});

            await game.Finish();

            var sb = new StringBuilder();
           

            for (int i=0; i< playerNicknames.Count; i++)
            {
                sb.Append(playerNicknames[i]);
                if (i<playerNicknames.Count - 1)
                    sb.Append(", ");
            }

            var playersN = sb.ToString();

            ViewData["Message"] = $"Game status is: {status}, Players: {playersN}";

            var  status2 = await game.GetStatus();

            ViewData["Message2"] = $"Game status is: {status2}";

            var gameInfo = await game.GetInfo();

            sb.Clear();
            sb.Append("Scores: ").Append("<br>");
            var scores = gameInfo.Scores.Values.ToList();
            for(int i=0; i < scores.Count;i++)
            {
                var score = scores[i];
                sb.Append(playerNicknames[i]).Append(" [ Kills: ").Append(score.Kills).Append(", Deaths: ").Append(score.Deaths).Append(", Assists: ")
                    .Append(score.Assistants).Append(" ]");
                sb.Append("<br>");
            }

            ViewData["Message3"] = sb.ToString();

            return View();
        }

       
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
