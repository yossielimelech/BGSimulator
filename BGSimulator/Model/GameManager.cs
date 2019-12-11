using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGSimulator.Model
{
    public class GameManager
    {
        Player player;

        public GameManager()
        {
            Initialize();
        }

        private void Initialize()
        {

            player = new Player();
        }
    }
}
