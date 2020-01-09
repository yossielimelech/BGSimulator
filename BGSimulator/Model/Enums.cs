using System;

namespace BGSimulator.Model
{
    [Flags]
    public enum Attribute
    {
        None = 0,
        Taunt = 1,
        DivineShield = 2,
        Poison = 4,
        WindFury = 8
    }

    [Flags]
    public enum MinionType
    {
        None = 0,
        Amalgam = Beast | Demon | Dragon | Mech | Murloc,
        Beast = 1,
        Demon = 2,
        Dragon = 4,
        Mech = 8,
        Murloc = 16,
        Neutral = 32,
        All = ~0
    }

    [Flags]
    public enum Keywords
    {
        None = 0,
        Targeted = 1,
        PlayerImmunity = 2,
        BattleCry = 4,
        Magnetic = 8,
        DeathRattle = 16,
        SpecialAttack = 32,
    }

    [Flags]
    public enum Adapt
    {
        OneOne = 1,
        ThreeHealth = 2,
        ThreeAttack = 4,
        DivineShield = 8,
        Taunt = 16,
        Poison = 32,
        DeathRattle = 64,
        Windfury = 128,
    }

    public enum Rarity
    {
        Classic,
        Common,
        Rare,
        Epic,
        Legendary
    }

    public enum Direction
    {
        Left = -1,
        InPlace = 0,
        Right = 1
    }

    public enum AuraType
    {
        BattleCry,
        Summon,
        Deathrattle
    }
}