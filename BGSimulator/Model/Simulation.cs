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
        const int NUM_OF_PLAYERS = 2;
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
            Console.WriteLine(string.Format(@"DEATH: {0} has died a horrible death", LastPlayerDied.Name));
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

            await StartBattlesAsync(MatchPlayers());
        }

        private async Task StartBattlesAsync(List<Player> fighters)
        {
            List<Task> battles = new List<Task>();
            List<Player> alreadyFighting = new List<Player>();

            foreach (var player in fighters)
            {
                if (!alreadyFighting.Contains(player))
                {
                    var task = Task.Run(() =>
                    {
                        StartBattle(player, player.CurrentMatch);
                    });

                    alreadyFighting.Add(player);
                    alreadyFighting.Add(player.CurrentMatch);
                    battles.Add(task);
                }
            }

            await Task.WhenAll(battles);
        }

        private void StartBattle(Player playerA, Player playerB)
        {
            Battle battle = new Battle() { PlayerA = playerA, PlayerB = playerB, PlayerABoard = playerA.Board.Clone(), PlayerBBoard = playerB.Board.Clone() };
            battle.Start();

        }

        private List<Player> MatchPlayers()
        {
            var matchedPlayers = players.Where(p => !p.IsDead).ToList();
            if (matchedPlayers.Count % 2 == 1)
            {
                matchedPlayers.Add(LastPlayerDied);
            }

            bool matched;
            do
            {

                matched = true;
                matchedPlayers.Shuffle();
                foreach (var player in matchedPlayers)
                {
                    player.CurrentMatch = null;
                }

                foreach (var player in matchedPlayers)
                {
                    if (player.CurrentMatch == null)
                    {
                        var currentMatch = matchedPlayers.FirstOrDefault(p => p.CurrentMatch == null && p != player && (matchedPlayers.Count == 2 || p.LastMatch != player));
                        if (currentMatch == null)
                        {
                            matched = false;
                            break;
                        }
                        player.CurrentMatch = currentMatch;
                        currentMatch.CurrentMatch = player;
                    }
                }
            } while (!matched);

            return matchedPlayers;
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
