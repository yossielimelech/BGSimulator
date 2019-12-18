using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGSimulator.Model
{
    public class TriggerParams
    {
        public IMinion Activator { get; set; }
        public IMinion Target { get; set; }
        public IMinion Summon { get; set; }
        public Board Board { get; set; }
        public Board RivalBoard { get; set; }
        public Player Player { get; set; }
        public int Index { get; set; }
        public int Damage { get; set; }
    }
}
