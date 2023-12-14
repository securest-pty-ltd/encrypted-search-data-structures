using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dekko.DataStructures.Crypto
{
    public class HNV1Hash
    {
        private const uint FNV_OFFFSET_BASIS_32 = 0x811C9DC5;
        private const uint FNV_PRIME_32 = 0x01000193;

        public static uint Hash(string data)
        {
            return Hash(System.Text.Encoding.UTF8.GetBytes(data));
        }

        public static uint Hash(byte[] data)
        {
            uint hash = FNV_OFFFSET_BASIS_32;
            for (int i = 0; i < data.Length; i++)
            {
                hash ^= data[i];
                hash *= FNV_PRIME_32;
            }
            return hash;
        }
    }
}