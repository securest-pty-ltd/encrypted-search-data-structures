using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

using DataStructures.HashFunctions;
using DataStructures.Models;
using DataStructures.Util;

namespace DataStructures.Factories
{
    public class BloomFilterFactory
    {
        public static BloomFilter CreateEmpty(BloomFilter baseFilter)
        {
            double bitsPerElement = baseFilter.BitsPerElement;
            long expectedElements = baseFilter.MaxElements;
            int hashes = baseFilter.NumberOfHashes;
            IHashFunction hashFunction = baseFilter.HashFunction;

            return new BloomFilter(bitsPerElement, expectedElements, hashes, hashFunction);
        }

        public static BloomFilter FromPath(string path, IHashFunction hashFunction)
        {
            var bloomFilterDTO = DiskUtil.ReadFromDiskJSON<BloomFilterDTO>(path);
            var bloomFilter = BloomFilterFromDTO(bloomFilterDTO, hashFunction);

            return bloomFilter;
        }

        public static void ToPath(BloomFilter bloomFilter, string path)
        {
            var bloomFilterDTO = BloomFilterToDTO(bloomFilter);
            DiskUtil.WriteToDiskJSON(path, bloomFilterDTO);
        }

        public static BloomFilterDTO BloomFilterToDTO(BloomFilter bloomFilter)
        {
            var filter = DiskUtil.BitArrayToBoolArray(bloomFilter.Filter);
            var bloomFilterDTO = new BloomFilterDTO
            {
                Id = bloomFilter.Id,
                NumberOfElements = bloomFilter.NumberOfElements,
                NumberOfBits = bloomFilter.NumberOfBits,
                NumberOfHashes = bloomFilter.NumberOfHashes,
                MaxElements = bloomFilter.MaxElements,
                BitsPerElement = bloomFilter.BitsPerElement,
                Filter = filter
            };

            return bloomFilterDTO;
        }

        public static BloomFilter BloomFilterFromDTO(BloomFilterDTO bloomFilterDTO, IHashFunction hashFunction)
        {
            var filter = DiskUtil.BitArrayToBoolArray(bloomFilterDTO.Filter);
            var bloomFilter = new BloomFilter(
                bitsPerElement: bloomFilterDTO.BitsPerElement,
                expctedElements: bloomFilterDTO.MaxElements,
                hashes: bloomFilterDTO.NumberOfHashes,
                elements: bloomFilterDTO.NumberOfElements,
                id: bloomFilterDTO.Id,
                filter: filter,
                hashFunction: hashFunction
            );

            return bloomFilter;
        }
    }
}