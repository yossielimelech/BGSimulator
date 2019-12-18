using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGSimulator.Model
{
    [Flags]
    public enum Attribute
    {
        None = 0,
        Taunt = 1,
        DivineShield = 2,
        Poison = 4,
        WindFury = 8,
        DeathRattle = 16
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
    public enum MinionTag
    {
        None = 0,
        Targeted = 1,
        PlayerImmunity = 2,
        BattleCry = 4,
        Magnetic = 8,
    }

    public enum Direction
    {
        Left = -1,
        InPlace = 0,
        Right = 1
    }
}
