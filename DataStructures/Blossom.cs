using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DataStructures.Factories;
using DataStructures.HashFunctions;
using DataStructures.Util;

namespace DataStructures
{
    public class Blossom
    {
        private readonly List<Guid>[] _filter;
        private readonly IHashFunction _hashFunction;
        private readonly long _bits;
        private readonly int _numberOfHashes;

        public Blossom(BloomFilter sampleFilter)
        {
            var emptyFilter = BloomFilterFactory.CreateEmpty(sampleFilter);

            _bits = emptyFilter.NumberOfBits;
            _numberOfHashes = emptyFilter.NumberOfHashes;
            _filter = new List<Guid>[_bits];
            _hashFunction = emptyFilter.HashFunction;

            for (int i = 0; i < _bits; i++)
            {
                _filter[i] = new List<Guid>();
            }
        }

        public void Insert(BloomFilter bloomFilter)
        {
            var bitArray = bloomFilter.Filter;

            for (int i = 0; i < bitArray.Length; i++)
            {
                if (bitArray[i])
                {
                    _filter[i].Add(bloomFilter.Id);
                }
            }
        }

        public List<Guid> FindMatches(string query)
        {
            byte[] bytes = DiskUtil.ToBytes(query);
            return FindMatches(bytes);
        }


        private List<Guid> FindMatches(byte[] bytes)
        {
            long[] hashes = GenerateHashes(bytes);

            List<Guid> filterIds = _filter[hashes[0]];

            for (int i = 1; i < hashes.Length; i++)
            {
                List<Guid> hashFilterIds = _filter[hashes[i]];
                List<Guid> newHashes = new();

                foreach (var id in filterIds)
                {
                    if (hashFilterIds.Contains(id))
                    {
                        newHashes.Add(id);
                    }
                }
            }

            return filterIds;
        }

        private long[] GenerateHashes(byte[] data)
        {
            var hashes = _hashFunction.GenerateHashes(data, _numberOfHashes, _bits);
            return hashes;
        }
    }
}