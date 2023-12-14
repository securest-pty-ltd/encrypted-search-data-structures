using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

namespace DataStructures
{
    public class JaggedBitArray
    {
        private const int MAX_INT = 2_147_483_647;

        private readonly BitArray[] _bitArray;

        public JaggedBitArray(long size)
        {
            int arrays = (int)(size / MAX_INT);
            int remainder = (int)(size % MAX_INT);

            if (remainder > 0)
            {
                arrays++;
            }

            _bitArray = new BitArray[arrays];

            for (int i = 0; i < arrays; i++)
            {
                if (i == arrays - 1)
                {
                    _bitArray[i] = new BitArray(remainder);
                }
                else
                {
                    _bitArray[i] = new BitArray(MAX_INT);
                }
            }
        }

        public void Set(long index, bool value)
        {
            int arr = (int)(index / MAX_INT);
            int pos = (int)(index - (arr * MAX_INT));

            _bitArray[arr][pos] = value;
        }

        public bool Get(long index)
        {
            int arr = (int)(index / MAX_INT);
            int pos = (int)(index - (arr * MAX_INT));

            return _bitArray[arr][pos];
        }
    }
}