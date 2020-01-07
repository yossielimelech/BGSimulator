using System;
using System.Collections.Generic;
using System.Linq;
using static BGSimulator.Utils.RandomUtils;

namespace BGSimulator.Model
{
    public class Board
    {
        private const int BOARD_SIZE = 7;

        public Board()
        {
            Initialize();
        }

        public List<IMinion> Graveyard { get; set; }
        public bool IsEmpty { get { return PlayedMinions.Count == 0; } }
        public bool IsFull { get { return PlayedMinions.Count == BOARD_SIZE; } }
        public List<IMinion> PlayedMinions { get; set; }
        public Player Player { get; set; }

        private int NextAttacker { get; set; }

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

                if (defendingMinion == null)
                    break;

                Console.WriteLine(string.Format(@"{0} {1} Is Attacking {2} {3}", Player.Name, attackingMinion.ToString(), defenderBoard.Player.Name, defendingMinion.ToString()));

                MinionAttack(attacker: attackingMinion, defender: defendingMinion, defenderBoard);
            }
        }

        public void ApplyAuras(Board rivalBoard)
        {
            foreach (var minion in PlayedMinions.Where(m => (m.Tags & MinionTag.Aura) != 0))
            {
                minion.OnApplyAura(new TriggerParams() { Activator = minion, Board = this, Player = Player, RivalBoard = rivalBoard });
            }
        }

        public void Buff(IMinion buffer, IMinion minion, int attack = 0, int health = 0, Attribute attributes = Attribute.None, Action<TriggerParams> deathRattle = null, bool aura = false)
        {
            int amount = buffer?.Level ?? 1;
            for (int i = 0; i < amount; i++)
            {
                if (!aura)
                {
                    minion.Attack += attack;
                    minion.Health += health;
                    minion.Attributes |= attributes;
                }
                else
                {
                    minion.AddAura(buffer, new Buff() { Attack = attack, Health = health, Attribute = attributes });
                }
                if (deathRattle != null)
                {
                    minion.OnDeath += deathRattle;
                }
            }
        }

        private void RemoveAuras(IMinion minion)
        {
            foreach (var m in PlayedMinions)
            {
                m.RemoveAura(minion);
            }

            if (BattleCryAura.ContainsKey(minion))
                BattleCryAura.Remove(minion);
        }

        public void BuffAdapt(Adapt adapt, int index)
        {
            int attack = 0;
            int health = 0;
            Attribute attr = Attribute.None;

            Action<TriggerParams> deathRattle = (tp) => { tp.Board.Summon("Plant", tp.Index, Direction.InPlace, 2); };

            switch (adapt)
            {
                case Adapt.DeathRattle:
                    attr |= Attribute.DeathRattle;
                    break;

                case Adapt.DivineShield:
                    attr |= Attribute.DivineShield;
                    break;

                case Adapt.OneOne:
                    attack++;
                    health++;
                    break;

                case Adapt.Poison:
                    attr |= Attribute.Poison;
                    break;

                case Adapt.Windfury:
                    attr |= Attribute.WindFury;
                    break;

                case Adapt.Taunt:
                    attr |= Attribute.Taunt;
                    break;

                case Adapt.ThreeAttack:
                    attack += 3;
                    break;

                case Adapt.ThreeHealth:
                    health += 3;
                    break;
            }

            BuffAllOfType(null, MinionType.Murloc, attack, health, attr, deathRattle);
        }

        public void BuffAdjacent(IMinion minion, int attack, int health, Attribute attributes)
        {
            var adjacents = GetAdjacentMinions(minion);
            foreach (var adj in adjacents)
            {
                Buff(minion, adj, attack, health, attributes);
            }
        }

        public void BuffAllOfType(IMinion buffer, MinionType type, int attack = 0, int health = 0, Attribute attributes = Attribute.None, Action<TriggerParams> deathRattle = null, bool temp = false)
        {
            foreach (var minion in PlayedMinions)
            {
                if ((type & minion.MinionType) != 0)
                {
                    Buff(buffer, minion, attack, health, attributes, deathRattle, temp);
                }
            }
        }

        public void BuffAllWithAttribute(IMinion buffer, Attribute attribute, int attack, int health)
        {
            foreach (var minion in PlayedMinions)
            {
                if (minion.Attributes.HasFlag(attribute))
                {
                    Buff(buffer, minion, attack, health);
                }
            }
        }

        public void PlayerTookDamage()
        {
            foreach (var minion in PlayedMinions)
            {
                minion.OnPlayerDamage(new TriggerParams() { Activator = minion, Board = this, Player = Player });
            }
        }

        public void BuffRandom(IMinion buffer, int attack = 0, int health = 0, Attribute attributes = Attribute.None, MinionType type = MinionType.All)
        {
            var buffee = GetRandomMinion(type);
            if (buffee == null)
                return;

            Buff(buffer, buffee, attack, health, attributes);
        }

        public void BuffRandomUnique(IMinion buffer, List<MinionType> buffedTypes, int attack, int health)
        {
            var uniqueBuffed = new List<IMinion>();

            foreach (var type in buffedTypes)
            {
                var buffed = GetRandomMinion(type, uniqueBuffed);
                if (buffed != null)
                    uniqueBuffed.Add(buffed);
            }

            foreach (var buffed in uniqueBuffed)
            {
                Buff(buffer, buffed, attack, health);
            }
        }

        public void CleaveAttack(IMinion activator, IMinion target)
        {
            var adjacent = GetAdjacentMinions(target);
            foreach (var minion in adjacent)
            {
                MinionTakeDamage(minion, activator.CurrentAttack);
            }
        }

        public Board Clone()
        {
            var board = this.MemberwiseClone() as Board;
            board.PlayedMinions = this.PlayedMinions.Select(m => m.Clone()).ToList();
            board.BattleCryAura.Clear();
            board.Graveyard = new List<IMinion>();
            return board;
        }

        public bool Controls(MinionType murloc, IMinion exclude = null)
        {
            return PlayedMinions.Any(m => (m.MinionType & MinionType.Murloc) != 0 && (exclude == null || m != exclude));
        }

        public IMinion GetNextAttacker()
        {
            if (NextAttacker >= PlayedMinions.Count)
                NextAttacker = 0;
            var minion = PlayedMinions[NextAttacker];
            NextAttacker++;
            return minion;
        }

        public IMinion GetRandomDefender()
        {
            if (IsEmpty)
                return null;

            var taunters = PlayedMinions.Where(m => m.Attributes.HasFlag(Attribute.Taunt)).ToArray();
            if (taunters.Any())
            {
                return taunters[RandomNumber(0, taunters.Length)];
            }

            return PlayedMinions[RandomNumber(0, PlayedMinions.Count)];
        }

        public IMinion GetRandomMinion(MinionType type = MinionType.All, List<IMinion> excludes = null)
        {
            var minions = PlayedMinions.Except(excludes ?? new List<IMinion>()).Where(m => (m.MinionType & type) != 0).ToArray();

            if (!minions.Any())
                return null;

            var minion = minions[RandomNumber(0, minions.Length)];
            return minion;
        }

        public List<IMinion> GetValidTargets(MinionType validTargets)
        {
            return PlayedMinions.Where(m => (validTargets & m.MinionType) != 0).ToList();
        }

        public void MinionAttack(IMinion attacker, IMinion defender, Board defenderBoard)
        {
            defenderBoard.MinionTakeDamage(defender, attacker.CurrentAttack);
            MinionTakeDamage(attacker, defender.CurrentAttack);

            attacker.OnAttack(new TriggerParams() { Activator = attacker, Target = defender, Board = this, RivalBoard = defenderBoard });
            OnMinionAttacked(attacker);

            ClearDeaths(defenderBoard);
            defenderBoard.ClearDeaths(this);
        }

        public void MinionTakeDamage(IMinion minion, int damage)
        {
            var damageResult = minion.TakeDamage(damage);
            if (damageResult.tookDamage)
            {
                minion.OnDamage(new TriggerParams() { Activator = minion, Board = this, Damage = damage });
                OnMinionTookDamage(minion);
            }

            if (damageResult.lostDivine)
            {
                OnMinionLostDivineShield(minion);
            }
        }

        public IMinion RemoveSmallestMinion()
        {
            var minion = PlayedMinions.OrderBy(m => m.Health).ThenBy(m => m.Attack).First();
            Remove(minion);
            return minion;
        }

        public void ShootRandomMinion(int damage, Board rivalBoard, int repeat = 1)
        {
            for (int i = 0; i < repeat; i++)
            {
                var minion = rivalBoard.GetRandomMinion();
                if(minion != null)
                {
                    rivalBoard.MinionTakeDamage(minion, damage);
                }
            }
        }

        public void Play(IMinion minion, int index = 0, IMinion target = null)
        {
            OnMinionSummon(minion, index);
            minion.OnApplyAura(new TriggerParams() { Activator = minion, Index = index, Board = this, Player = Player });
            int auraLevel = BattleCryAura.Select(b => b.Value).DefaultIfEmpty().Max() + 1;
            PlayedMinions.Insert(index, minion);
            for (int j = 0; j < auraLevel; j++)
            {
                minion.OnPlayed(new TriggerParams() { Activator = minion, Index = index, Target = target, Board = this, Player = Player });
            }


        }

        public void Remove(IMinion minion)
        {
            RemoveAuras(minion);
            PlayedMinions.Remove(minion);
        }

        public void RoundEnd()
        {
            for (int i = 0; i < PlayedMinions.Count; i++)
            {
                var minion = PlayedMinions[i];

                for (int j = 0; j < minion.Level; j++)
                {
                    minion.OnTurnEnd(new TriggerParams() { Activator = minion, Index = i, Board = this, Player = Player });
                }
            }
        }

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

        public void Summon(string minionName, int index, Direction direction = Direction.Right, int amount = 1, bool golden = false)
        {
            for (int i = 0; i < amount; i++)
            {
                if (IsFull)
                {
                    return;
                }

                var summoned = Pool.Instance.GetFreshCopy(minionName);
                PlayedMinions.Insert(index + (int)direction, summoned);
                OnMinionSummon(summoned, index);
            }
        }

        public void Summon(List<IMinion> minions, int index, Direction direction, bool golden = false)
        {
            foreach (var minion in minions)
            {
                Summon(minion.Name, index, direction, golden: golden);
            }
        }

        public void SummonFromGraveyard(MinionType type, int index, Direction direction = Direction.InPlace, int amount = 1)
        {
            var revive = Graveyard.Where(m => m.MinionType == type).Take(amount);
            foreach (var minion in revive)
            {
                Summon(minion.Name, index, direction);
            }
        }

        public void TryMagnet(IMinion magnetic, int index)
        {
            if (index++ > PlayedMinions.Count)
                return;

            var minion = PlayedMinions[index];
            if ((magnetic.ValidTargets & minion.MinionType) != 0)
            {
                minion.Attack += magnetic.Attack;
                minion.Health += magnetic.Health;
                minion.Attributes |= magnetic.Attributes;
                Remove(magnetic);
                Pool.Instance.Return(magnetic);
            }
        }

        private void ClearDeaths(Board rivalBoard)
        {
            Dictionary<IMinion, int> deaths = new Dictionary<IMinion, int>();

            for (int i = 0; i < PlayedMinions.Count; i++)
            {
                var minion = PlayedMinions[i];
                if (minion.IsDead)
                {
                    deaths[minion] = i;
                    int index = PlayedMinions.IndexOf(minion);
                    Remove(minion);
                    Graveyard.Add(minion);
                }
            }

            foreach (var kv in deaths)
            {
                kv.Key.OnDeath(new TriggerParams() { Activator = kv.Key, Board = this, RivalBoard = rivalBoard, Index = kv.Value });
                OnMinionDied(kv.Key, rivalBoard);
            }
        }

        private List<IMinion> GetAdjacentMinions(IMinion minion)
        {
            List<IMinion> adjacent = new List<IMinion>();
            int index = PlayedMinions.IndexOf(minion);
            int right = index - 1;
            int left = index + 1;

            if (right >= 0)
            {
                adjacent.Add(PlayedMinions[right]);
            }
            if (left < PlayedMinions.Count)
            {
                adjacent.Add(PlayedMinions[left]);
            }

            return adjacent;
        }

        private void Initialize()
        {
            PlayedMinions = new List<IMinion>();
        }

        private void OnMinionAttacked(IMinion attacker)
        {
            foreach (var minion in PlayedMinions)
            {
                minion.OnMinionAttacked(new TriggerParams() { Activator = minion, Board = this, Target = attacker });
            }
        }

        private void OnMinionDied(IMinion deadMinion, Board rivalBoad)
        {
            for (int i = 0; i < PlayedMinions.Count; i++)
            {
                PlayedMinions[i].OnMinionDied(new TriggerParams() { Activator = PlayedMinions[i], Target = deadMinion, Board = this, RivalBoard = rivalBoad });
            }

            rivalBoad.ClearDeaths(this);
        }

        private void OnMinionLostDivineShield(IMinion lostDivine)
        {
            foreach (var minion in PlayedMinions)
            {
                minion.OnMinionLostDivineShield(new TriggerParams() { Activator = minion, Board = this, Target = lostDivine });
            }
        }

        private void OnMinionSummon(IMinion summoned, int index)
        {
            foreach (IMinion minion in PlayedMinions)
            {
                minion.OnMinionSummon(new TriggerParams() { Activator = minion, Index = index, Summon = summoned, Board = this, Player = Player });
            }
        }

        private void OnMinionTookDamage(IMinion tookDamage)
        {
            foreach (var minion in PlayedMinions.Where(m => m != tookDamage))
            {
                minion.OnMinionDamaged(new TriggerParams() { Activator = minion, Board = this, Target = tookDamage });
            }
        }

        Dictionary<IMinion, int> BattleCryAura = new Dictionary<IMinion, int>();
        public void SetBattlecryAura(IMinion activator)
        {
            BattleCryAura.Add(activator, activator.Level);
        }
    }
}