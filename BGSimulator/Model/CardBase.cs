using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGSimulator.Model
{
    public class CardBase : ICard
    {
        public int Cost { get; set; } = 1;
        public string Name { get; set; }
        public Action<TriggerParams> OnPlayed { get; set; } = delegate { };
        public Rarity Rarity { get; set; } = Rarity.Common;
    }
}
