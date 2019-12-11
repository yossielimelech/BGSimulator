using System;

namespace BGSimulator.Model
{
    public interface IMinion
    {
        MinionType MinionType { get; set; }
        int Attack { get; set; }
        Attribute Attributes { get; set; }
        int Health { get; set; }
        int Level { get; set; }
        MinionTier MinionTier { get; set; }
        string Name { get; set; }
        int NumberOfCopies { get; set; }
        Action<IMinion, Board, int> OnDeath { get; set; }
        Action<IMinion, IMinion, Board, int> OnMinionSummon { get; set; }
        Action<IMinion, Board, int> OnPlayed { get; set; }
        Action<IMinion, Board, int> OnTurnEnd { get; set; }
        Action<IMinion, Board, int> OnTurnStart { get; set; }
        bool PoolMinion { get; set; }

        IMinion Clone(bool fullClone = false);
    }
}