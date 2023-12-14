using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace DataStructures.Crypto
{
    public class Crytography
    {
        public static byte[] GenerateSalt()
        {
            byte[] salt = new byte[16];
            using var random = RandomNumberGenerator.Create();
            random.GetBytes(salt);

            return salt;
        }
    }
}