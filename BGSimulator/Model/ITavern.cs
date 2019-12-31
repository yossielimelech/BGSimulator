using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGSimulator.Model
{
    public interface ITavern
    {
        void Roll(Player player, bool free = false);
        void Mulligen(Player player);
        void Sell(IMinion minionBase);
        IMinion CreateGolden(Player player, IEnumerable<IMinion> tripple);
    }
}
