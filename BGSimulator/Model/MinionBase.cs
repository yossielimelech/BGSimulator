using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGSimulator.Model
{
    public class MinionBase : IMinion
    {
        public MinionType MinionType { get; set; }
        public int NumberOfCopies { get; set; }
        public string Name { get; set; }
        public MinionTier MinionTier { get; set; } = MinionTier.Ranks[1];
        public Attribute Attributes { get; set; } = Attribute.None;
        public MinionTag Tags { get; set; } = MinionTag.None;
        public MinionType ValidTargets { get; set; } = MinionType.All;

        public Rarity Rarity { get; set; } = Rarity.Common;
        public int Level { get; set; } = 1;
        public int Health { get; set; }
        public int Attack { get; set; }
        public bool PoolMinion { get; set; } = true;
        public Action<TriggerParams> OnPlayed { get; set; } = delegate { };
        public Action<TriggerParams> OnDeath { get; set; } = delegate { };
        public Action<TriggerParams> OnTurnStart { get; set; } = delegate { };
        public Action<TriggerParams> OnTurnEnd { get; set; } = delegate { };
        public Action<TriggerParams> OnMinionSummon { get; set; } = delegate { };
        public Action<TriggerParams> OnAttack { get; set; } = delegate { };
        public Action<TriggerParams> OnMinionAttacked { get; set; } = delegate { };
        public Action<TriggerParams> OnMinionDied { get; set; } = delegate { };
        public Action<TriggerParams> OnDamage { get; set; } = delegate { };
        public Action<TriggerParams> OnMinionDamaged { get; set; } = delegate { };
        public Action<TriggerParams> OnMinionLostDivineShield { get; set; } = delegate { };

        public bool IsDead { get { return Health <= 0; } }


        public IMinion Clone(bool fullClone = false)
        {
            return this.MemberwiseClone() as IMinion;
        }

        public void DoAttack(IMinion minion)
        {
            minion.TakeDamage(Attack);
            TakeDamage(minion.Attack);
        }

        public (bool tookDamage, bool lostDivine) TakeDamage(int damage)
        {
            if (damage > 0 && (Attributes & Attribute.DivineShield) != 0)
            {
                Attributes &= ~Attribute.DivineShield;
                return (false, true);
            }

            Health -= damage;

            if (damage > 0)
                return (true, false);

            return (false, false);
        }

        public override string ToString()
        {
            return string.Format("[{0}][{1}][{2}]", Name, Attack, Health);
        }
    }
}
