using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataStructures.Models
{
    public class BloomFilterDTO
    {
        public Guid Id { get; set; }
        public long NumberOfElements { get; set; }
        public long NumberOfBits { get; set; }
        public int NumberOfHashes { get; set; }
        public long MaxElements { get; set; }
        public double BitsPerElement { get; set; }
        public required bool[] Filter { get; set; }
    }
}