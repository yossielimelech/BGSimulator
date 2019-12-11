using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGSimulator.Model
{
    public class MinionTier
    {
        public int Tier { get; set; } = 1;
        public int NumberOfCopies { get; set; } = 1;

        public static Dictionary<int, MinionTier> Ranks = new Dictionary<int, MinionTier>()
        {
            {1, new MinionTier{Tier = 1, NumberOfCopies = 18} },
            {2, new MinionTier{Tier = 2, NumberOfCopies = 15} },
            {3, new MinionTier{Tier = 3, NumberOfCopies = 13} },
            {4, new MinionTier{Tier = 4, NumberOfCopies = 11} },
            {5, new MinionTier{Tier = 5, NumberOfCopies = 9} },
            {6, new MinionTier{Tier = 6, NumberOfCopies = 6} },
        };
    }
}
