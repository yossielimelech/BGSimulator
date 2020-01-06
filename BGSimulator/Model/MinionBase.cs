using System;
using System.Collections.Generic;

namespace BGSimulator.Model
{
    public class MinionBase : CardBase, IMinion
    {
        public MinionType MinionType { get; set; }
        public int NumberOfCopies { get; set; }
        public MinionTier MinionTier { get; set; } = MinionTier.Ranks[1];
        public Attribute Attributes { get; set; } = Attribute.None;
        public MinionTag Tags { get; set; } = MinionTag.None;
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
                return currHealth;
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
        public Action<TriggerParams> OnSummonSelf { get; set; } = delegate { };
        public Action<TriggerParams> OnMinionSummon { get; set; } = delegate { };
        public Action<TriggerParams> OnAttack { get; set; } = delegate { };
        public Action<TriggerParams> OnMinionAttacked { get; set; } = delegate { };
        public Action<TriggerParams> OnMinionDied { get; set; } = delegate { };
        public Action<TriggerParams> OnDamage { get; set; } = delegate { };
        public Action<TriggerParams> OnMinionDamaged { get; set; } = delegate { };
        public Action<TriggerParams> OnMinionLostDivineShield { get; set; } = delegate { };
        public Action<TriggerParams> OnBoardChanged { get; set; } = delegate { };
        public Action<TriggerParams> OnPlayerDamage { get; set; } = delegate { };
        public Dictionary<IMinion, Buff> TempBuffs { get; set; } = new Dictionary<IMinion, Buff>();

        public bool IsDead { get { return CurrentHealth <= 0; } }


        public IMinion Clone(bool fullClone = false)
        {
            return this.MemberwiseClone() as IMinion;
        }

        public (bool tookDamage, bool lostDivine) TakeDamage(int damage)
        {
            if (damage == 0)
            {
                return (false, false);
            }

            if ((Attributes & Attribute.DivineShield) != 0)
            {
                Attributes &= ~Attribute.DivineShield;
                return (false, true);
            }

            Health -= damage;

            return (true, false);
        }

        public void RemoveTempBuff(IMinion minion)
        {
            Buff buff;
            if (TempBuffs.TryGetValue(minion, out buff))
            {
                TempBuffs.Remove(minion);
                Health += buff.Health;
                Attack += buff.Attack;
            }
        }

        public override string ToString()
        {
            return string.Format("[{0}][{1}][{2}]", Name, CurrentAttack, CurrentHealth);
        }
    }
}