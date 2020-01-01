using System;
using System.Collections.Generic;

namespace BGSimulator.Model
{
    public interface IMinion : ICard
    {
        int Attack { get; set; }
        Attribute Attributes { get; set; }
        int Health { get; set; }
        bool IsDead { get; }
        int Level { get; set; }
        MinionTier MinionTier { get; set; }
        MinionType MinionType { get; set; }
        int NumberOfCopies { get; set; }
        Action<TriggerParams> OnAttack { get; set; }
        Action<TriggerParams> OnBoardChanged { get; set; }
        Action<TriggerParams> OnDamage { get; set; }
        Action<TriggerParams> OnDeath { get; set; }
        Action<TriggerParams> OnMinionAttacked { get; set; }
        Action<TriggerParams> OnMinionDamaged { get; set; }
        Action<TriggerParams> OnMinionDied { get; set; }
        Action<TriggerParams> OnMinionLostDivineShield { get; set; }
        Action<TriggerParams> OnMinionSummon { get; set; }
        Action<TriggerParams> OnTurnEnd { get; set; }
        Action<TriggerParams> OnTurnStart { get; set; }
        bool PoolMinion { get; set; }
        MinionTag Tags { get; set; }
        MinionType ValidTargets { get; set; }
        IMinion Clone(bool fullClone = false);
        void DoAttack(IMinion minion);
        (bool tookDamage, bool lostDivine) TakeDamage(int damage);
    }
}