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

        public Action<Player> OnDeath = delegate { };
        const int MAX_HAND_SIZE = 10;
        const int MAX_SHOP_LEVEL = 6;
        const int PLAYER_MAX_HEALTH = 40;

        public Player()
        {
            Board = new Board() { Player = this };
        }

        public Board Board { get; set; }
        public ITavern BobsTavern { get; set; }
        public IBrain Brain { get; set; }
        public Player CurrentMatch { get; set; }
        public int Gold { get; set; } = 3;
        public List<ICard> Hand { get; set; } = new List<ICard>();
        public int Health { get; set; } = PLAYER_MAX_HEALTH;
        public bool IsDead { get => Health <= 0; }
        public Player LastMatch { get; set; }
        public List<string> MinionsPlayedThisGame { get; private set; } = new List<string>();
        public int MissingHealth { get { return PLAYER_MAX_HEALTH - Health; } }
        public string Name { get; set; }
        public int ShopLevel { get; set; } = 1;
        public int ShopLevelupPrice { get; set; }
        public List<IMinion> ShopOffer { get; set; } = new List<IMinion>();
        public int Top { get; set; }
        public bool Freeze { get; set; }

        public void AddToHand(ICard minion)
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

        public void PlayRound()
        {
            RoundStart();
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
                bool canPlay = Hand.Any();
                bool shopOfferEmpty = !ShopOffer.Any();

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

                if(canRoll && shopOfferEmpty)
                {
                    Roll();
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
                    if(tableFull)
                        Sell();
                    PlayHand();
                    continue;
                }


                done = !canBuy && !canLevel;
            }

            TryFreeze();

            Board.RoundEnd();
        }

        private void RoundStart()
        {
            foreach (var minion in Hand.Where(h => h is IMinion).Cast<IMinion>())
            {
                minion.OnTurnStart(new TriggerParams() { Activator = minion, Player = this });
            }
        }

        private void TryFreeze()
        {
            if (!ShopOffer.Any())
                return;

            Freeze = RandomNumber(1, 6) == 5;

            if (!Freeze)
            {
                BobsTavern.Mulligen(this);
            }
            else
            {
                Console.WriteLine(string.Format(@"Round {1}: {0} Froze the tavern", Name, Simulation.Instance.Round));
            }
        }

        public void TakeDamage(int damage, bool always = false)
        {
            if (damage ==0 || (!always && Board.PlayedMinions.Any(m => m.Keywords.HasFlag(Keywords.PlayerImmunity))))
                return;

            Health -= damage;

            Board.PlayerTookDamage();

            if (Health <= 0)
            {
                OnDeath(this);
            }

        }

        private void Buy()
        {
            if (Gold < 3 || Hand.Count == MAX_HAND_SIZE || !ShopOffer.Any())
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
            var tripple = Hand.Where(h => h is IMinion).Cast<IMinion>().Concat(Board.PlayedMinions).Where(m => m.Level == 1).GroupBy(m => m.Name).FirstOrDefault(g => g.Count() == 3)?.Select(m => m);

            if (tripple != null)
            {
                foreach (var minion in tripple)
                {
                    if (Hand.Contains(minion))
                        Hand.Remove(minion);
                    if (Board.PlayedMinions.Contains(minion))
                        Board.Remove(minion);
                }

                var golden = BobsTavern.CreateGolden(this, tripple);

                AddToHand(golden);

                Console.WriteLine(string.Format(@"Round {2}: {0} created a tripple of minion {1}", Name, tripple.First()?.Name, Simulation.Instance.Round));
            }
        }

        private IMinion ChooseRandomMinion(List<IMinion> minions)
        {
            return minions[RandomNumber(0, minions.Count)];
        }

        private IMinion ChooseRandomTarget(List<IMinion> targets)
        {
            return targets[RandomNumber(0, targets.Count)];
        }

        private IEnumerable<Adapt> GetAdaptOptions()
        {
            var adapts = Enum.GetValues(typeof(Adapt)).Cast<Adapt>().ToList();
            adapts.Shuffle();
            return adapts.Take(3);
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

        private bool Play(IMinion minion, int index = 0, IMinion target = null)
        {
            if (Board.IsFull)
                return false;

            MinionsPlayedThisGame.Add(minion.Name);
            Board.Play(minion, index, target);
            Console.WriteLine(string.Format(@"Round {2}: {0} played {1}", Name, minion.Name, Simulation.Instance.Round));

            return true;
        }

        private void PlayCard(ICard card)
        {
            Hand.Remove(card);

            card.OnPlayed(new TriggerParams() { Player = this });
        }

        private void PlayHand()
        {
            for (int i = 0; i < Hand.Count; i++)
            {
                if (Hand[i] is IMinion)
                {
                    PlayMinion(Hand[i] as IMinion);
                }
                else
                {
                    PlayCard(Hand[i]);
                }
            }
        }

        private void PlayMinion(IMinion minion)
        {
            IMinion target = null;
            IMinion playMinion = minion;

            if(minion.Contained != null)
            {
                playMinion = minion.Contained;
            }


            if (playMinion.Keywords.HasFlag(Keywords.Targeted))
            {
                var targets = Board.GetValidTargets(minion.ValidTargets);
                if (targets.Any())
                {
                    target = ChooseRandomTarget(targets);
                }
            }

            if (Play(playMinion, target: target))
            {
                minion.Contained = null;
                Hand.Remove(minion);
            }
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

        private void Sell()
        {
            if (Board.IsEmpty)
                return;

            var minion = Board.RemoveSmallestMinion();
            BobsTavern.Sell(minion);

            Console.WriteLine(string.Format(@"Round {2}: {0} has sold a minion {1}", Name, minion.Name, Simulation.Instance.Round));
        }

        public override string ToString()
        {
            return ($"{Name}({Health})");
        }
    }
}
