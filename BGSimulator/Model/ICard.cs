using System;

namespace BGSimulator.Model
{
    public interface ICard
    {
        int Cost { get; set; }
        string Name { get; set; }
        Action<TriggerParams> OnPlayed { get; set; }
        Rarity Rarity { get; set; }
    }
}