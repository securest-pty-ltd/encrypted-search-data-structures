using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

namespace DataStructures
{
    public class LongBitArray
    {
        private const int MAX_INT = 214_7483_647;
        private readonly BitArray _arrayOne;
        private readonly BitArray? _arrayTwo;
        public LongBitArray(long size)
        {
            if (size > MAX_INT)
            {
                _arrayOne = new BitArray(MAX_INT);
                _arrayTwo = new BitArray((int)(size - MAX_INT));
            }
            else
            {
                _arrayOne = new BitArray((int)size);
            }
        }

        public void Set(long index, bool value)
        {
            if (_arrayTwo != null && index > MAX_INT)
            {
                _arrayTwo[(int)index - MAX_INT] = value;
            }
            else
            {
                _arrayOne[(int)index] = value;
            }
        }

        public bool Get(long index)
        {
            if (_arrayTwo != null && index > MAX_INT)
            {
                return _arrayTwo[(int)index - MAX_INT];
            }
            else
            {
                return _arrayOne[(int)index];
            }
        }
    }
}