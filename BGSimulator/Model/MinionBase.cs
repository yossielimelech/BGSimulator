using System;
using System.Collections.Generic;

namespace BGSimulator.Model
{
    public class MinionBase : CardBase, IMinion
    {
        public MinionBase()
        {
            damageTaken = 0;
        }

        private int damageTaken;

        public MinionType MinionType { get; set; }
        public int NumberOfCopies { get; set; }
        public MinionTier MinionTier { get; set; } = MinionTier.Ranks[1];
        public Attribute Attributes { get; set; } = Attribute.None;
        public Keywords Keywords { get; set; } = Keywords.None;
        public MinionType ValidTargets { get; set; } = MinionType.All;
        public int Level { get; set; } = 1;
        public int Health { get; set; }
        public int Attack { get; set; }

        public int CurrentHealth
        {
            get
            {
                int currHealth = Health;
                foreach (var buff in TempBuffs.Values)
                {
                    currHealth += buff.Health;
                }
                return currHealth - damageTaken;
            }
        }

        public int CurrentAttack
        {
            get
            {
                int currAttack = Attack;
                foreach (var buff in TempBuffs.Values)
                {
                    currAttack += buff.Attack;
                }
                return currAttack;
            }
        }

        public bool PoolMinion { get; set; } = true;
        public Action<TriggerParams> OnDeath { get; set; } = delegate { };
        public Action<TriggerParams> OnTurnStart { get; set; } = delegate { };
        public Action<TriggerParams> OnTurnEnd { get; set; } = delegate { };
        public Action<TriggerParams> OnApplyAura { get; set; } = delegate { };
        public Action<TriggerParams> OnMinionSummon { get; set; } = delegate { };
        public Action<TriggerParams> OnAttack { get; set; } = delegate { };
        public Action<TriggerParams> OnMinionAttacked { get; set; } = delegate { };
        public Action<TriggerParams> OnMinionDied { get; set; } = delegate { };
        public Action<TriggerParams> OnDamage { get; set; } = delegate { };
        public Action<TriggerParams> OnMinionDamaged { get; set; } = delegate { };
        public Action<TriggerParams> OnMinionLostDivineShield { get; set; } = delegate { };
        public Action<TriggerParams> OnBoardChanged { get; set; } = delegate { };
        public Action<TriggerParams> OnPlayerDamage { get; set; } = delegate { };
        public Dictionary<IMinion, Buff> TempBuffs { get; set; }
        public Action<TriggerParams> OnBattlefieldChanged { get; set; } = delegate { };
        public Func<TriggerParams, IMinion> OnAquireTargets { get; set; } = delegate { return null; };
        public bool IsDead { get { return CurrentHealth <= 0; } }

        public IMinion Contained { get; set; }

        public IMinion Clone(bool keepBuffs = false)
        {
            var clone = this.MemberwiseClone() as IMinion;
            if (!keepBuffs)
            {
                clone.TempBuffs = new Dictionary<IMinion, Buff>();
            }
            return clone;
        }

        public (bool tookDamage, bool lostDivine, bool overkill, bool killed) TakeDamage(int damage)
        {
            if (damage == 0)
            {
                return (false, false, false, false);
            }

            if ((Attributes & Attribute.DivineShield) != 0)
            {
                Attributes &= ~Attribute.DivineShield;
                return (false, true, false, false);
            }

            damageTaken += damage;

            return (true, false, CurrentHealth < 0, CurrentHealth <= 0);
        }

        public void RemoveAura(IMinion minion)
        {
            Buff buff;
            if (TempBuffs.TryGetValue(minion, out buff))
            {
                damageTaken -= buff.Health;
                if (damageTaken < 0)
                    damageTaken = 0;
                TempBuffs.Remove(minion);
            }
        }

        public override string ToString()
        {
            return string.Format("[{0}][{1}][{2}]", Name, CurrentAttack, CurrentHealth);
        }

        public void AddAura(IMinion buffer, Buff buff)
        {
            TempBuffs[buffer] = buff;
        }
    }
}