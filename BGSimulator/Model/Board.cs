using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BGSimulator.Utils.RandomUtils;

namespace BGSimulator.Model
{
    public class Board
    {
        const int BOARD_SIZE = 7;

        public List<IMinion> PlayedMinions { get; set; }

        public Board()
        {
            Initialize();
        }

        public Player Player { get; set; }

        private void Initialize()
        {
            PlayedMinions = new List<IMinion>();
        }

        public void Summon(string minionName, int index, Direction direction = Direction.Right, int amount = 1)
        {
            if (IsFull)
            {
                return;
            }

            var summoned = Pool.Instance.GetCopy(minionName);
            PlayedMinions.Insert(direction == Direction.Left ? index : index + 1, summoned);
            OnMinionSummon(summoned, index);
        }

        public bool IsFull { get { return PlayedMinions.Count == BOARD_SIZE; } }

        public bool IsEmpty { get { return PlayedMinions.Count == 0; } }

        public void RoundStart()
        {
            for (int i = 0; i < PlayedMinions.Count; i++)
            {
                var minion = PlayedMinions[i];

                for (int j = 0; j < minion.Level; j++)
                {
                    minion.OnTurnStart(new TriggerParams() { Activator = minion, Index = i, Board = this, Player = Player });
                }
            }
        }

        public void Play(IMinion minion, int index = 0, IMinion target = null)
        {
            OnMinionSummon(minion, index);
            PlayedMinions.Insert(index, minion);
            for (int j = 0; j < minion.Level; j++)
            {
                minion.OnPlayed(new TriggerParams() { Activator = minion, Index = index, Target = target, Board = this, Player = Player });
            }
        }

        private void OnMinionSummon(IMinion summoned, int index)
        {
            foreach (IMinion minion in PlayedMinions)
            {
                minion.OnMinionSummon(new TriggerParams() { Activator = minion, Index = index, Summon = summoned, Board = this, Player = Player });
            }
        }

        public IMinion RemoveRandomMinion()
        {
            int index = RandomNumber(0, PlayedMinions.Count);
            var minion = PlayedMinions[index];
            PlayedMinions.Remove(minion);
            return minion;
        }

        public void BuffRandom(int attack = 0, int health = 0, Attribute attributes = Attribute.None, MinionType type = MinionType.All)
        {
            var buffee = GetRandomMinion(type);
            if (buffee == null)
                return;

            Buff(buffee, attack, health, attributes);
        }

        public void Buff(IMinion minion, int attack = 0, int health = 0, Attribute attributes = Attribute.None)
        {
            minion.Attack += attack;
            minion.Health += health;
            minion.Attributes |= attributes;
        }

        public IMinion GetRandomMinion(MinionType type = MinionType.All)
        {
            var minions = PlayedMinions.Where(m => m != null && (m.MinionType.HasFlag(type))).ToArray();

            if (!minions.Any())
                return null;

            var minion = minions[RandomNumber(0, minions.Length)];
            return minion;
        }

        public List<IMinion> GetValidTargets(MinionType validTargets)
        {
            return PlayedMinions.Where(m => validTargets.HasFlag(m.MinionType)).ToList();
        }
    }
}
