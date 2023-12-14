using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataStructures.HashFunctions
{
    public interface IHashFunction
    {
        public long[] GenerateHashes(byte[] data, int hashes, long range);
    }
}