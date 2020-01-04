using System.Collections.Generic;

namespace BGSimulator.Model
{
    public class BobsTavern : ITavern
    {
        public int Round { get; set; } = 1;
        public Pool Pool { get; set; }

        public IMinion CreateGolden(Player player, IEnumerable<IMinion> tripple)
        {
            return Pool.CreateGolden(tripple);
        }

        public void Mulligen(Player player)
        {
            Pool.Return(player.ShopOffer);
        }

        public void Roll(Player player, bool free = false)
        {
            if ((!free && player.Gold == 0))
                return;

            if(player.Freeze)
            {
                player.Freeze = false;
                return;
            }

                if (!free)
                player.Gold--;


            player.ShopOffer = Pool.Roll(AmountToDeal(player.ShopLevel), player.ShopLevel);
        }

        public void Sell(IMinion minion)
        {
            Pool.Return(minion);
        }

        private int AmountToDeal(int level)
        {
            return (level / 2) + 3;
        }
    }
}