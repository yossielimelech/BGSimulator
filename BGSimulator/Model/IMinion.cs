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
        bool PoolMinion { get; set; }

        IMinion Clone(bool fullClone = false);
    }
}