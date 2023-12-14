using System;
using System.Collections;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

using DataStructures.Crypto;

namespace DataStructures.HashFunctions
{
    public class SHA256Hash : IHashFunction
    {
        private readonly byte[] _salt;

        public SHA256Hash()
        {
            _salt = GenerateSalt();
        }

        public long[] GenerateHashes(byte[] data, int hashes, long range)
        {
            long[] result = new long[hashes];
            byte[] saltedData = data.Concat(_salt).ToArray();
            byte[] bytes = SHA256.HashData(saltedData);

            // max hashes = 8
            for (int i = 0; i < hashes; i++)
            {
                var hash = BitConverter.ToInt32(bytes, i * 4);
                var x = Math.Abs(hash) % range;
                result[i] = x;
            }
 
            return result;
        }

        private static byte[] GenerateSalt()
        {
            byte[] salt = new byte[16];
            using var random = RandomNumberGenerator.Create();
            random.GetBytes(salt);

            return salt;
        }
    }
}