using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BGSimulator.Utils
{
    public static class RandomUtils
    {
        public static readonly Random random = new Random();
        private static readonly object syncLock = new object();

        public static int RandomNumber(int min, int max)
        {
            lock (syncLock)
            {
                return random.Next(min, max);
            }
        }

        public static object[] Shuffle(object[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                int rnd = RandomNumber(0, array.Length);
                var temp = array[rnd];
                array[rnd] = array[i];
                array[i] = temp;
            }

            return array;
        }
    }
}
