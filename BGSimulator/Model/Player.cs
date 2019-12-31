using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BGSimulator.Utils.RandomUtils;

namespace BGSimulator.Model
{
    public class Player
    {

        const int MAX_SHOP_LEVEL = 6;
        const int MAX_HAND_SIZE = 10;
        const int PLAYER_MAX_HEALTH = 40;

        public string Name { get; set; }
        public int ShopLevel { get; set; } = 1;
        public int ShopLevelupPrice { get; set; }
        public int Gold { get; set; } = 3;
        public int Health { get; set; } = PLAYER_MAX_HEALTH;
        public bool IsDead { get => Health <= 0; }
        public List<IMinion> Hand { get; set; } = new List<IMinion>();
        public Board Board { get; set; }
        public List<IMinion> ShopOffer { get; set; } = new List<IMinion>();
        public ITavern BobsTavern { get; set; }
        public IBrain Brain { get; set; }
        public int Top { get; set; }
        public List<string> MinionsPlayedThisGame { get; private set; } = new List<string>();
        public Player CurrentMatch { get; set; }
        public Player LastMatch { get; set; }
        public int MissingHealth { get { return PLAYER_MAX_HEALTH - Health; } }

        public Action<Player> OnDeath = delegate { };


        public Player()
        {
            Board = new Board() { Player = this };
        }

        public void PlayRound()
        {
            Board.RoundStart();
            bool done = false;

            while (!done)
            {
                bool canBuy = Gold > 2 && Hand.Count < MAX_HAND_SIZE;
                bool canLevel = ShopLevel < MAX_SHOP_LEVEL && Gold >= ShopLevelupPrice;
                bool canBuyAndLevel = canBuy && ShopLevel < MAX_SHOP_LEVEL && Gold > ShopLevelupPrice + 2;
                bool canRoll = Gold > 0;
                bool tableFull = Board.IsFull;
                bool shopHasDouble = ShopOffer.GroupBy(m => m.Name).Any(g => g.Count() > 1);
                bool canMakeTriple = Hand.Concat(Board.PlayedMinions).Concat(ShopOffer).GroupBy(m => m.Name).Any(g => g.Count() > 2);
                bool canSell = Hand.Any() || Board.PlayedMinions.Any();
                bool canPlay = Hand.Any() && !Board.IsFull;

                if (Simulation.Instance.Round == 2)
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

            Board.RoundEnd();

            BobsTavern.Mulligen(this);
        }

        private void Sell()
        {
            if (Board.IsEmpty)
                return;

            var minion = Board.RemoveRandomMinion();
            BobsTavern.Sell(minion);

            Console.WriteLine(string.Format(@"Round {2}: {0} has sold a minion {1}", Name, minion.Name, Simulation.Instance.Round));
        }

        private void PlayHand()
        {
            for (int i = 0; i < Hand.Count; i++)
            {
                IMinion target = null;
                var minion = Hand[i];
                if (minion.Tags.HasFlag(MinionTag.Targeted))
                {
                    var targets = Board.GetValidTargets(minion.ValidTargets);
                    if (targets.Any())
                    {
                        target = ChooseRandomTarget(targets);
                    }
                }

                if (Play(minion, target: target))
                {
                    Hand.Remove(minion);
                }
            }
        }


        public void TakeDamage(int damage)
        {
            if (Board.PlayedMinions.Any(m => m.Tags.HasFlag(MinionTag.PlayerImmunity)))
                return;

            Health -= damage;

            if (Health <= 0)
            {
                OnDeath(this);
            }
        }

        private IMinion ChooseRandomTarget(List<IMinion> targets)
        {
            return targets[RandomNumber(0, targets.Count)];
        }

        private bool Play(IMinion minion, int index = 0, IMinion target = null)
        {
            if (Board.IsFull)
                return false;

            MinionsPlayedThisGame.Add(minion.Name);
            Board.Play(minion, index, target);
            Console.WriteLine(string.Format(@"Round {2}: {0} played {1}", Name, minion.Name, Simulation.Instance.Round));

            return true;
        }


        private void Roll()
        {
            if (Gold > 0)
            {
                BobsTavern.Mulligen(this);
                BobsTavern.Roll(this);

                Console.WriteLine(string.Format(@"Round {1}: {0} rolled", Name, Simulation.Instance.Round));
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

                Console.WriteLine(string.Format(@"Round {2}: {0} has leveled up to level {1}", Name, ShopLevel, Simulation.Instance.Round));
            }
        }

        private void Buy()
        {
            if (Gold < 3 || Hand.Count == MAX_HAND_SIZE)
                return;

            Gold -= 3;
            var minion = ShopOffer.OrderByDescending(m => m.Health + m.Attack).FirstOrDefault();

            Hand.Add(minion);
            ShopOffer.Remove(minion);

            CheckForTripple();

            Console.WriteLine(string.Format(@"Round {2}: {0} has bought a minion {1}", Name, minion.Name, Simulation.Instance.Round));

        }

        private void CheckForTripple()
        {
            var tripple = Hand.Concat(Board.PlayedMinions).Where(m => m.Level == 1).GroupBy(m => m.Name).FirstOrDefault(g => g.Count() == 3)?.Select(m => m);

            if (tripple != null)
            {
                foreach (var minion in tripple)
                {
                    if (Hand.Contains(minion))
                        Hand.Remove(minion);
                    if (Board.PlayedMinions.Contains(minion))
                        Board.PlayedMinions.Remove(minion);
                }

                var golden = BobsTavern.CreateGolden(this, tripple);

                AddToHand(golden);

                Console.WriteLine(string.Format(@"Round {2}: {0} created a tripple of minion {1}", Name, tripple.First()?.Name, Simulation.Instance.Round));
            }
        }

        private void AddToHand(IMinion minion)
        {
            if (Hand.Count == MAX_HAND_SIZE)
                return;

            Hand.Add(minion);
        }

        public Adapt ChooseAdapt()
        {
            var adaptOptions = GetAdaptOptions();
            return adaptOptions.First();
        }

        private IEnumerable<Adapt> GetAdaptOptions()
        {
            var adapts = Enum.GetValues(typeof(Adapt)).Cast<Adapt>().ToList();
            adapts.Shuffle();
            return adapts.Take(3);
        }

        public void ChooseDiscover(List<IMinion> minions)
        {
            if (Hand.Count == MAX_HAND_SIZE)
            {
                Pool.Instance.Return(minions);
                return;
            }

            var minion = ChooseRandomMinion(minions);
            minions.Remove(minion);
            Pool.Instance.Return(minions);
            Hand.Add(minion);
        }

        private IMinion ChooseRandomMinion(List<IMinion> minions)
        {
            return minions[RandomNumber(0, minions.Count)];
        }
    }
}
