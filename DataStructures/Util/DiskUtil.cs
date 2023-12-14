using System;
using System.Collections;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace DataStructures.Util
{
    public class DiskUtil
    {
        public static ulong BitArrayToULong(BitArray bitArray)
        {
            if (bitArray.Length > 64)
            {
                throw new ArgumentException("BitArray length must be less than or equal to 64");
            }

            ulong result = 0;

            for (int i = 0; i < bitArray.Length; i++)
            {
                if (bitArray[i])
                {
                    result |= 1UL << i;
                }
            }

            return result;
        }

        public static BitArray ULongToBitArray(ulong value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            return new BitArray(bytes);
        }

        public static bool[] BitArrayToBoolArray(BitArray bitArray)
        {
            bool[] result = new bool[bitArray.Length];

            for (int i = 0; i < bitArray.Length; i++)
            {
                result[i] = bitArray[i];
            }

            return result;
        }

        public static BitArray BitArrayToBoolArray(bool[] boolArray)
        {
            BitArray result = new BitArray(boolArray.Length);

            for (int i = 0; i < boolArray.Length; i++)
            {
                result[i] = boolArray[i];
            }

            return result;
        }

        // public static void WriteToDiskBin(string pathToWrite, object objectToWrite)
        // {
        //     BinaryFormatter binaryFormatter = new BinaryFormatter();
        //     MemoryStream memoryStream = new MemoryStream();

        //     binaryFormatter.Serialize(memoryStream, objectToWrite);
        //     byte[] bytes = memoryStream.ToArray();

        //     using var stream = File.Open(pathToWrite, FileMode.Create);
        //     using var writer = new BinaryWriter(stream, Encoding.UTF8, false);

        //     writer.Write(bytes);
        // }


        public static void WriteToDiskJSON(string pathToWrite, object objectToWrite)
        {
            var settings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                Formatting = Formatting.Indented
            }; // settings probs not needed

            string jsonString = JsonConvert.SerializeObject(objectToWrite, settings);

            try
            {
                File.WriteAllText(pathToWrite, jsonString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static T ReadFromDiskJSON<T>(string path)
        {
            try
            {
                string jsonString = File.ReadAllText(path);

                var settings = new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                    Formatting = Formatting.Indented
                };

                var deserializedObject = JsonConvert.DeserializeObject<T>(jsonString, settings);

                if (deserializedObject == null)
                {
                    throw new InvalidOperationException($"Deserialization resulted in a null {nameof(T)} object. Check the JSON file format and the {nameof(T)} class structure.");
                }

                return deserializedObject;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public static byte[] ToBytes<T>(T element)
        {
            var elementString = element?.ToString();

            if (elementString == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            byte[] bytes = Encoding.UTF8.GetBytes(elementString);

            return bytes;
        }
    }
}