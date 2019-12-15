using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BGSimulator.Utils.RandomUtils;

namespace BGSimulator.Model
{
    public class Simulation
    {
        const int NUM_OF_PLAYERS = 8;
        const int MAX_GOLD = 10;
        Player LastPlayerDied;
        BobsShop shop;
        Pool pool;
        Player[] players;

        public int Round { get; private set; } = 1;
        private Simulation()
        {
            Initialize();
        }

        static Simulation instance;
        public static Simulation Instance
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }

                instance = new Simulation();
                return instance;
            }
        }

        private void Initialize()
        {
            pool = Pool.Instance;
            shop = new BobsShop() { Pool = pool };
            CreatePlayers();
        }

        private void CreatePlayers()
        {
            players = new Player[NUM_OF_PLAYERS];
            for (int i = 0; i < NUM_OF_PLAYERS; i++)
            {
                players[i] = new Player() { Name = "Player " + (i + 1), Gold = 3, ShopLevel = 1, ShopLevelupPrice = 6, Shop = shop };
                players[i].OnDeath += PlayerDied;
            }
        }

        object deathLock = new object();
        private void PlayerDied(Player player)
        {
            LastPlayerDied = player;
        }

        public async void Start()
        {
            while (!GameOver())
            {
                await BeginRoundAsync();
                Round++;
            }
        }

        private async Task BeginRoundAsync()
        {
            List<Task> runningPlayers = new List<Task>();
            foreach (var player in players)
            {
                if (!player.IsDead)
                {
                    player.LastMatch = player.CurrentMatch;
                    player.CurrentMatch = null;

                    var task = Task.Run(() =>
                    {
                        shop.Roll(player, free: true);
                        player.Gold = GetGoldPerRound();
                        player.ShopLevelupPrice--;
                        player.PlayRound();
                    });

                    runningPlayers.Add(task);
                }
            }

            await Task.WhenAll(runningPlayers);

            if (GameOver())
                return;

            // Start battle phase
            MatchPlayers();


        }

        private void MatchPlayers()
        {
            var livePlayers = players.Where(p => !p.IsDead).ToList();
            if (livePlayers.Count % 2 == 1)
            {
                livePlayers.Add(LastPlayerDied);
            }

            livePlayers.Shuffle();

            foreach (var player in livePlayers)
            {
                if (player.CurrentMatch == null)
                {
                    player.CurrentMatch = livePlayers.First(p => p.CurrentMatch == null && p != player && p.LastMatch != player);
                    player.CurrentMatch.CurrentMatch = player;
                }
            }
        }

        private int GetGoldPerRound()
        {
            return Round + 2 < MAX_GOLD ? Round + 2 : MAX_GOLD;
        }

        private bool GameOver()
        {
            return players.Select(p => !p.IsDead).ToList().Count == 1;
        }
    }
}
