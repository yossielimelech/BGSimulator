using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGSimulator.Model
{
    public class Player
    {
        const int MAX_SHOP_LEVEL = 6;
        const int MAX_HAND_SIZE = 10;
        public string Name { get; set; }
        public int ShopLevel { get; set; } = 1;
        public int ShopLevelupPrice { get; set; }
        public int Gold { get; set; } = 3;
        public int Health { get; set; } = 40;
        public bool IsDead { get => Health <= 0; }
        public IMinion[] Hand { get; set; } = new IMinion[MAX_HAND_SIZE];
        public Board Board { get; set; } = new Board();
        public List<IMinion> ShopOffer { get; set; } = new List<IMinion>();
        public IShop Shop { get; set; }
        public IBrain Brain { get; set; }

        public Player()
        {
        }

        public void PlayRound()
        {
            Board.RoundStart();
            bool done = false;
            
            while (!done)
            {
                bool canBuy = Gold > 2 && Hand.Any(s => s == null);
                bool canLevel = ShopLevel < MAX_SHOP_LEVEL && Gold >= ShopLevelupPrice;
                bool canBuyAndLevel = canBuy && ShopLevel < MAX_SHOP_LEVEL && Gold > ShopLevelupPrice + 2;
                bool canRoll = Gold > 0;
                bool tableFull = Board.IsFull;
                bool shopHasDouble = ShopOffer.GroupBy(m => m.Name).Any(g => g.Count() > 1);
                bool canMakeTriple = Hand.Concat(Board.PlayedMinions).Concat(ShopOffer).Where(m => m != null).GroupBy(m => m.Name).Any(g => g.Count() > 2);
                bool canSell = Hand.Any(m => m != null) || Board.PlayedMinions.Any();
                bool canPlay = Hand.Any(m => m != null) && !tableFull;

                if (Game.Instance.Round == 2)
                {
                    LevelUp();
                    break;
                }

                if (canBuyAndLevel)
                {
                    LevelUp();
                    continue;
                }

                if (canBuy)
                {
                    Buy();
                    continue;
                }

                if (canRoll)
                {
                    Roll();
                    continue;
                }

                if (canPlay)
                {
                    PlayHand();
                    continue;
                }

                if (tableFull)
                {
                    Sell();
                    continue;
                }

                done = !canBuy && !canLevel;
            }

            Shop.Mulligen(this);
        }

        private void Sell()
        {
            if (Board.IsEmpty)
                return;

            var minion = Board.SellRandomMinion();
            Shop.Sell(minion);

            Console.WriteLine(string.Format(@"Round {2}: {0} has sold a minion {1}", Name, minion.Name, Game.Instance.Round));
        }

        private void PlayHand()
        {
            if (!Hand.Any(m => m == null))
                return;

            for (int i = 0; i < Hand.Length; i++)
            {
                if (Hand[i] != null)
                {
                    Play(i);
                }
            }

        }

        private void Play(int index, int target = -1)
        {
            if (Board.IsFull)
                return;

            var minion = Hand[index];

            if (Board.Play(Hand[index]))
            {
                Hand[index] = null;

                Console.WriteLine(string.Format(@"Round {2}: {0} played {1}", Name, minion.Name, Game.Instance.Round));
            }
        }

        private void Roll()
        {
            if (Gold > 0)
            {
                Shop.Mulligen(this);
                Shop.Roll(this);

                Console.WriteLine(string.Format(@"Round {1}: {0} rolled", Name, Game.Instance.Round));
            }
        }

        private void LevelUp()
        {
            if (ShopLevel == MAX_SHOP_LEVEL)
                return;

            if (Gold >= ShopLevelupPrice)
            {
                ShopLevel++;
                Gold -= ShopLevelupPrice;
                ShopLevelupPrice = ShopLevel + 5;

                Console.WriteLine(string.Format(@"Round {2}: {0} has leveled up to level {1}", Name, ShopLevel, Game.Instance.Round));
            }
        }

        private void Buy()
        {
            if (Gold < 3 || !Hand.Any(s => s == null))
                return;

            Gold -= 3;
            var minion = ShopOffer.OrderByDescending(m => m.Health + m.Attack).FirstOrDefault();

            for (int i = 0; i < Hand.Length; i++)
            {
                if (Hand[i] == null)
                {
                    Hand[i] = minion;
                    break;
                }
            }

            ShopOffer.Remove(minion);

            Console.WriteLine(string.Format(@"Round {2}: {0} has bought a minion {1}", Name, minion.Name, Game.Instance.Round));

        }

    }
}
