using System;
using System.Collections.Generic;

namespace BGSimulator.Model
{
    public interface IMinion : ICard
    {
        int Attack { get; set; }
        Attribute Attributes { get; set; }
        int CurrentHealth { get; }
        int CurrentAttack { get; }
        int Health { get; set; }
        bool IsDead { get; }
        int Level { get; set; }
        MinionTier MinionTier { get; set; }
        MinionType MinionType { get; set; }
        int NumberOfCopies { get; set; }
        Action<TriggerParams> OnAttack { get; set; }
        Action<TriggerParams> OnBattlefieldChanged { get; set; }
        Action<TriggerParams> OnBoardChanged { get; set; }
        Action<TriggerParams> OnDamage { get; set; }
        Action<TriggerParams> OnDeath { get; set; }
        Action<TriggerParams> OnMinionAttacked { get; set; }
        Action<TriggerParams> OnMinionDamaged { get; set; }
        Action<TriggerParams> OnMinionDied { get; set; }
        Action<TriggerParams> OnMinionLostDivineShield { get; set; }
        Action<TriggerParams> OnMinionSummon { get; set; }
        Action<TriggerParams> OnApplyAura { get; set; }
        Action<TriggerParams> OnTurnEnd { get; set; }
        Action<TriggerParams> OnTurnStart { get; set; }
        Action<TriggerParams> OnPlayerDamage { get; set; }
        Func<TriggerParams, IMinion> OnAquireTargets { get; set; }
        Dictionary<IMinion, Buff> TempBuffs { get; set; }
        IMinion Contained { get; set; }
        bool PoolMinion { get; set; }
        Keywords Keywords { get; set; }
        MinionType ValidTargets { get; set; }
        IMinion Clone(bool keepBuffs = false);
        (bool tookDamage, bool lostDivine, bool overkill, bool killed) TakeDamage(int damage);
        void AddAura(IMinion buffer, Buff buff);
        void RemoveAura(IMinion minion);
    }
}