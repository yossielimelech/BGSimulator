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

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
