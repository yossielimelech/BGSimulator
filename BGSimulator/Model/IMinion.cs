using System;

namespace BGSimulator.Model
{
    public interface IMinion
    {
        MinionType MinionType { get; set; }
        int Attack { get; set; }
        Attribute Attributes { get; set; }
        MinionTag Tags { get; set; }
        MinionType ValidTargets { get; set; }
        Rarity Rarity { get; set; }
        int Cost { get; set; }
        int Health { get; set; }
        int Level { get; set; }
        MinionTier MinionTier { get; set; }
        string Name { get; set; }
        int NumberOfCopies { get; set; }
        Action<TriggerParams> OnDeath { get; set; }
        Action<TriggerParams> OnMinionSummon { get; set; }
        Action<TriggerParams> OnPlayed { get; set; }
        Action<TriggerParams> OnTurnEnd { get; set; }
        Action<TriggerParams> OnTurnStart { get; set; }
        Action<TriggerParams> OnAttack { get; set; }
        Action<TriggerParams> OnMinionAttacked { get; set; }
        Action<TriggerParams> OnMinionDied { get; set; }
        Action<TriggerParams> OnDamage { get; set; }
        Action<TriggerParams> OnMinionDamaged { get; set; }
        Action<TriggerParams> OnMinionLostDivineShield { get; set; }
        Action<TriggerParams> OnBoardChanged { get; set; }
        bool PoolMinion { get; set; }
        bool IsDead { get; }

        IMinion Clone(bool fullClone = false);
        (bool tookDamage, bool lostDivine) TakeDamage(int damage);
        void DoAttack(IMinion minion);
    }
}