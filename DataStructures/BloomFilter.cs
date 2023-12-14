using System;
using System.Collections;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using DataStructures.HashFunctions;
using DataStructures.Util;

using Newtonsoft.Json;

namespace DataStructures
{
    [Serializable]
    public class BloomFilter
    {
        private readonly BitArray _filter;
        private readonly long _bits; // should this be a uint?
        private long _elements;

        public IHashFunction HashFunction { get; }

        public int NumberOfHashes { get; }

        public long MaxElements { get; }

        public double BitsPerElement { get; }

        public long NumberOfBits => _bits;
        public long NumberOfElements => _elements;
        public BitArray Filter => _filter;

        public Guid Id { get; } = Guid.NewGuid();

        public BloomFilter(double bitsPerElement, long expctedElements, int hashes, IHashFunction hashFunction)
        {
            MaxElements = expctedElements;
            BitsPerElement = bitsPerElement;
            NumberOfHashes = hashes;
            _bits = OptimalBits(expctedElements, BitsPerElement);
            _elements = 0;

            if (_bits > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(bitsPerElement), "bitsPerElement is too large for given expectedElements");

            _filter = new BitArray((int)_bits);
            HashFunction = hashFunction;
        }

        public BloomFilter(double falsePositiveProbability, long expctedElements, IHashFunction hashFunction)
        {
            MaxElements = expctedElements;
            BitsPerElement = OptimalBitsPerElement(falsePositiveProbability);
            NumberOfHashes = OptimalHashFunctions(expctedElements, falsePositiveProbability);
            _bits = OptimalBits(expctedElements, BitsPerElement);
            _elements = 0;

            if (_bits > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(expctedElements), "expctedElements is too large for desired false positive rate");

            _filter = new BitArray((int)_bits);
            HashFunction = hashFunction;
        }

        public BloomFilter(
            double bitsPerElement,
            long expctedElements,
            int hashes,
            long elements,
            Guid id,
            BitArray filter,
            IHashFunction hashFunction)
        {
            MaxElements = expctedElements;
            BitsPerElement = bitsPerElement;
            NumberOfHashes = hashes;
            Id = id;
            _bits = filter.Length;
            _elements = elements;

            if (_bits > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(bitsPerElement), "bitsPerElement is too large for given expectedElements");

            _filter = filter;
            HashFunction = hashFunction;
        }

        public override string ToString()
        {
            return $"BloomFilter(elements: {_elements}, expectedElements: {MaxElements}, bits: {_bits}, bitsPerElement: {BitsPerElement:0.00}, hashes: {NumberOfHashes}, falsePositiveProbability:{FalsePositiveProbability():0.00}, expectedFalsePositiveProbability: {ExpectedFalsePositiveProbability():0.00})";
        }

        public string ToStringValue()
        {
            var filterString = FilterToString();
            var percentFull = FilterPercentFull();

            return $"{percentFull}%";
            // return $"({percentFull}%) {filterString}";
        }

        public string FilterPercentFull()
        {
            var bitsOn = 0;
            foreach (bool bit in _filter)
            {
                if (bit)
                {
                    bitsOn++;
                }
            }

            var percentFull = (double)bitsOn / _filter.Length * 100;
            return $"{percentFull:0.00}";
        }

        public string FilterToString()
        {
            var sb = new StringBuilder();
            foreach (bool bit in _filter)
            {
                sb.Append(bit ? "1" : "0");
            }

            sb.Append($" ({Id})");

            return sb.ToString();
        }

        public void Add<T>(T element)
        {
            byte[] bytes = DiskUtil.ToBytes(element);
            Add(bytes);
        }

        public void Add(byte[] bytes)
        {
            long[] hashes = GenerateHashes(bytes);

            foreach (int hash in hashes)
            {
                if (!_filter.Get(hash))
                {
                    _filter.Set(hash, true);
                }
            }

            _elements++;
        }

        public bool Contains<T>(T element)
        {
            byte[] bytes = DiskUtil.ToBytes(element);
            return Contains(bytes);
        }

        public bool Contains(byte[] bytes)
        {
            long[] hashes = GenerateHashes(bytes);

            foreach (int hash in hashes)
            {
                if (!_filter.Get(hash))
                {
                    return false;
                }
            }

            return true;
        }

        public void Clear()
        {
            _filter.SetAll(false);
            _elements = 0;
        }

        public void Or(BloomFilter filter)
        {
            if (_bits != filter._bits)
            {
                throw new ArgumentException("Both Bloom filters must have the same number of bits");
            }

            _filter.Or(filter._filter);
            _elements += filter._elements;
        }

        public int HammingDistance(BloomFilter otherFilter)
        {
            var currentFilter = this;

            if (currentFilter._bits != otherFilter._bits)
            {
                throw new ArgumentException("Both Bloom filters must have the same number of bits");
            }

            int distance = 0;

            for (int i = 0; i < currentFilter._bits; i++)
            {
                if (currentFilter._filter[i] != otherFilter._filter[i])
                {
                    distance++;
                }
            }

            return distance;
        }

        public bool IsFull()
        {
            var isFull = _filter.Cast<bool>().Contains(true);
            Console.WriteLine($"Is full: {isFull} ({Id})");
            return isFull;
        }

        public double ExpectedFalsePositiveProbability()
        {
            int k = NumberOfHashes;
            long n = MaxElements;
            long m = _bits;

            return FalsePositiveProbability(k, n, m);
        }

        public double FalsePositiveProbability()
        {
            int k = NumberOfHashes;
            long n = _elements;
            long m = _bits;

            return FalsePositiveProbability(k, n, m);
        }

        public double FalsePositiveProbability(int hashFunctions, long elementsInserted, long bits)
        {
            double k = hashFunctions;
            double n = elementsInserted;
            double m = bits;

            double p = Math.Pow(1.0 - Math.Exp(-k * n / m), k);
            return p;
        }

        private long OptimalBits(long expctedElements, double bitsPerElement)
        {
            double m = expctedElements * bitsPerElement;
            return (long)Math.Ceiling(m);
        }

        private double OptimalBitsPerElement(double falsePositiveProbability)
        {
            double p = falsePositiveProbability;

            double m = -Math.Log(p) / Math.Pow(Math.Log(2), 2);
            return (long)Math.Ceiling(m);
        }

        private int OptimalHashFunctions(long expectedElements, double falsePositiveProbability)
        {
            long n = expectedElements;
            double p = falsePositiveProbability;

            double m = -n * Math.Log(p) / Math.Pow(Math.Log(2), 2);
            double k = m / n * Math.Log(2);

            return (int)Math.Ceiling(k);
        }

        private long[] GenerateHashes(byte[] data)
        {
            var hashes = HashFunction.GenerateHashes(data, NumberOfHashes, _bits);
            return hashes;
        }
    }
}