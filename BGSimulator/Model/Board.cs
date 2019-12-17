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

            var summoned = Pool.Instance.GetFreshCopy(minionName);
            PlayedMinions.Insert(index + (int)direction, summoned);
            OnMinionSummon(summoned, index);
        }

        int NextAttacker { get; set; }
        public IMinion GetNextAttacker()
        {
            if (NextAttacker >= PlayedMinions.Count)
                NextAttacker = 0;
            var minion = PlayedMinions[NextAttacker];
            NextAttacker++;
            return minion;
        }

        public bool IsFull { get { return PlayedMinions.Count == BOARD_SIZE; } }

        public void Attack(Board defenderBoard)
        {
            IMinion attackingMinion = GetNextAttacker();

            if (attackingMinion.Attack == 0)
            {
                return;
            }

            int attacks = attackingMinion.Attributes.HasFlag(Attribute.WindFury) ? 2 : 1;

            for (int i = 0; i < attacks; i++)
            {
                IMinion defendingMinion = defenderBoard.GetRandomDefender();

                Console.WriteLine(string.Format(@"{0} {1} Is Attacking {2} {3}", Player.Name, attackingMinion.ToString(), defenderBoard.Player.Name, defendingMinion.ToString()));

                MinionAttack(attacker: attackingMinion, defender: defendingMinion, defenderBoard);

            }
        }
        public void MinionAttack(IMinion attacker, IMinion defender, Board defenderBoard)
        {
            defenderBoard.MinionTakeDamage(defender, attacker.Attack);
            MinionTakeDamage(attacker, defender.Attack);

            attacker.OnAttack(new TriggerParams() { Activator = attacker, Target = defender, Board = defenderBoard });

            ClearDeaths();
            defenderBoard.ClearDeaths();
        }

        private void ClearDeaths()
        {
            Dictionary<IMinion, int> deaths = new Dictionary<IMinion, int>();

            for (int i = 0; i < PlayedMinions.Count; i++)
            {
                var minion = PlayedMinions[i];
                if (minion.IsDead)
                {
                    deaths[minion] = i;
                    int index = PlayedMinions.IndexOf(minion);
                    PlayedMinions.Remove(minion);
                }
            }

            foreach (var kv in deaths)
            {
                kv.Key.OnDeath(new TriggerParams() { Activator = kv.Key, Board = this, Index = kv.Value });
                OnMinionDied(kv.Key);
            }
        }

        public void MinionTakeDamage(IMinion minion, int damage)
        {
            var tookDamage = minion.TakeDamage(damage);
            if (tookDamage)
            {
                minion.OnDamage(new TriggerParams() { Activator = minion, Board = this, Damage = damage });
                OnMinionTookDamage(minion);
            }
        }

        private void OnMinionDied(IMinion deadMinion)
        {
            foreach (var minion in PlayedMinions.Where(m => m != deadMinion))
            {
                minion.OnMinionDied(new TriggerParams() { Activator = minion, Target = deadMinion, Board = this });
            }
        }



        private void OnMinionTookDamage(IMinion tookDamage)
        {
            foreach (var minion in PlayedMinions.Where(m => m != tookDamage))
            {
                minion.OnMinionDamaged(new TriggerParams() { Activator = minion, Board = this, Target = tookDamage });
            }
        }

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

        public void Remove(IMinion defendingMinion)
        {
            int i = PlayedMinions.IndexOf(defendingMinion);
            if (i < NextAttacker)
            {
                NextAttacker--;
            }
            PlayedMinions.Remove(defendingMinion);
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

        public void BuffAllOfType(MinionType type, int attack = 0, int health = 0, Attribute attributes = Attribute.None)
        {
            foreach (var minion in PlayedMinions)
            {
                if ((type & minion.MinionType) != 0)
                {
                    Buff(minion, attack, health, attributes);
                }
            }
        }

        public IMinion GetRandomMinion(MinionType type = MinionType.All)
        {
            var minions = PlayedMinions.Where(m => (m.MinionType & type) != 0).ToArray();

            if (!minions.Any())
                return null;

            var minion = minions[RandomNumber(0, minions.Length)];
            return minion;
        }

        public IMinion GetRandomDefender()
        {
            var taunters = PlayedMinions.Where(m => m.Attributes.HasFlag(Attribute.Taunt)).ToArray();
            if (taunters.Any())
            {
                return taunters[RandomNumber(0, taunters.Length)];
            }

            return PlayedMinions[RandomNumber(0, PlayedMinions.Count)];
        }

        public List<IMinion> GetValidTargets(MinionType validTargets)
        {
            return PlayedMinions.Where(m => (validTargets & m.MinionType) != 0).ToList();
        }

        public Board Clone()
        {
            var borad = this.MemberwiseClone() as Board;
            borad.PlayedMinions = this.PlayedMinions.Select(m => m.Clone()).ToList();
            return borad;
        }
    }
}
