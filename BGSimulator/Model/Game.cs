using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGSimulator.Model
{
    public class Game
    {
        const int NUM_OF_PLAYERS = 8;
        const int MAX_GOLD = 10;

        Board board;
        BobsShop shop;
        Pool pool;
        Player[] players;

        public int Round { get; private set; } = 1;
        private Game()
        {
            Initialize();
        }

        static Game instance;
        public static Game Instance
        {
            get
            {
                if(instance != null)
                {
                    return instance;
                }

                instance = new Game();
                return instance;
            }
        }

        private void Initialize()
        {
            pool = new Pool();
            shop = new BobsShop() { Pool = pool };
            board = new Board() { Pool = pool };
            CreatePlayers();
        }

        private void CreatePlayers()
        {
            players = new Player[NUM_OF_PLAYERS];
            for (int i = 0; i < NUM_OF_PLAYERS; i++)
            {
                players[i] = new Player() { Name = "Player " + (i + 1), Gold = 3, ShopLevel = 1, ShopLevelupPrice = 6, Shop = shop };
            }
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
                    var task = Task.Run(() =>
                    {
                        shop.Roll(player, free: true);
                        player.Gold = GetGoldPerRound();
                        player.ShopLevelupPrice--;
                        player.PlayRound();
                    });

                    runningPlayers.Add(task);
                }
                break;
            }

            await Task.WhenAll(runningPlayers);
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
