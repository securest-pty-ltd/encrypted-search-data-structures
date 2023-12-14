using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataStructures.Factories
{
    public class BloofiFactory
    {
        public static Bloofi CreateEmpty(int order, BloomFilter baseFilter)
        {
            return new Bloofi(order, baseFilter);
        }
    }
}