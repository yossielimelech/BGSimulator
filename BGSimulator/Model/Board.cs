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
            foreach (var minion in PlayedMinions.Where(m => m != null))
            {
                for (int i = 0; i < minion.Level; i++)
                {
                    minion.OnTurnStart(minion, this, 0);
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
                        minion.OnPlayed(minion, this, i);
                    }
                    OnMinionSummon(minion, i);
                    return true;
                }
            }

            return false;
        }

        private void OnMinionSummon(IMinion minion, int index)
        {
            foreach (IMinion active in ActiveMinions)
            {
                active.OnMinionSummon(active, minion, this, index);
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

        public IMinion GetRandomMinion()
        {
            return PlayedMinions.FirstOrDefault(m => m != null);
        }
    }
}
