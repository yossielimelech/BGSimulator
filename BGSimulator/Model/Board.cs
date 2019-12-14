using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGSimulator.Model
{
    public class Board
    {
        const int BOARD_SIZE = 7;

        public IMinion[] PlayedMinions { get; set; }
        public Pool Pool { get; set; }

        public Board()
        {
            Initialize();
        }

        public Player Player { get; set; }

        private void Initialize()
        {
            PlayedMinions = new IMinion[BOARD_SIZE];
        }

        public void Summon(string minionName, int index, int amount = 1)
        {
            if (PlayedMinions[index] != null)
            {
                return;
            }

            var minion = Pool.GetCopy(minionName);

            PlayedMinions[index] = minion;
            OnMinionSummon(minion, index);
        }

        public bool IsFull { get { return !PlayedMinions.Any(s => s == null); } }

        public bool IsEmpty { get { return !PlayedMinions.Any(s => s != null); } }

        public void RoundStart()
        {
            for (int i = 0; i < BOARD_SIZE; i++)
            {
                if (PlayedMinions[i] != null)
                {
                    var minion = PlayedMinions[i];

                    for (int j = 0; j < minion.Level; j++)
                    {
                        minion.OnTurnStart(new TriggerParams() { Activator = minion, Index = i, Board = this, Player = Player });
                    }
                }
            }
        }

        public bool Play(IMinion minion)
        {
            for (int i = 0; i < BOARD_SIZE; i++)
            {
                if (PlayedMinions[i] == null)
                {
                    PlayedMinions[i] = minion;
                    for (int j = 0; j < minion.Level; j++)
                    {
                        minion.OnPlayed(new TriggerParams() { Activator = minion, Index = i, Board = this, Player = Player });
                    }
                    OnMinionSummon(minion, i);
                    return true;
                }
            }

            return false;
        }

        public void Buff(IMinion minion, int attack = 0,int health = 0, Attribute attributes = Attribute.None)
        {
            minion.Attack += attack;
            minion.Health += health;
            minion.Attributes |= attributes;
        }

        private void OnMinionSummon(IMinion minion, int index)
        {
            foreach (IMinion active in ActiveMinions)
            {
                active.OnMinionSummon(new TriggerParams() { Activator = minion, Board = this, Player = Player });
            }
        }

        private IEnumerable<IMinion> ActiveMinions { get => PlayedMinions.Where(m => m != null).ToList(); }

        public IMinion SellRandomMinion()
        {
            var minions = PlayedMinions.Where(m => m != null).Select((m, i) => new { minion = m, index = i }).ToArray();
            Random r = new Random();
            int index = minions[r.Next(0, minions.Length)].index;
            var toSell = PlayedMinions[index];
            PlayedMinions[index] = null;
            return toSell;
        }

        public void BuffRandom(int attack = 0, int health = 0, Attribute attributes = Attribute.None, MinionType type = MinionType.All)
        {
            var buffee = GetRandomMinion(type);
            if (buffee == null)
                return;

            Buff(buffee, attack, health, attributes);
        }

        public IMinion GetRandomMinion(MinionType type = MinionType.All)
        {
            return PlayedMinions.FirstOrDefault(m => m != null && (m.MinionType.HasFlag(type)));
        }
    }
}
