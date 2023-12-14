using System;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

using DataStructures;
using DataStructures.Factories;
using DataStructures.HashFunctions;
using DataStructures.Util;

using Newtonsoft.Json;

class Program
{

    static void Main()
    {
        BitArray bitArray = new(10);
        bitArray.Set(0, true);
        bitArray.Set(2, true);
        bitArray.Set(4, true);
        bitArray.Set(6, true);
        bitArray.Set(8, true);

        var hashFunction = new SHA256Hash();

        var numOfElements = 100;
        var numOfWords = 10;
        int maxFilters = 10;
        var baseFilter = new BloomFilter(0.01, numOfElements, hashFunction);
        Console.WriteLine(baseFilter.ToString());
        var filterList = new List<BloomFilter>();
        // var bloofi = new Bloofi(2, BloomFilterFactory.CreateEmpty(baseFilter));
        var blossom = new Blossom(baseFilter);

        var addedWords = new Dictionary<string, Guid>();

        string filePath = "./Assets/words.txt";

        using StreamReader reader = new(filePath);

        int filters = 0;
        string? line = reader.ReadLine();


        while (line != null && filters < maxFilters)
        {
            var bloomFilter = BloomFilterFactory.CreateEmpty(baseFilter);
            for (int i = 0; i < numOfWords; i++)
            {
                bloomFilter.Add(line);

                if (line != null)
                {
                    addedWords[line] = bloomFilter.Id;
                }

                line = reader.ReadLine();
            }

            filterList.Add(bloomFilter);
            blossom.Insert(bloomFilter);
            // Console.WriteLine($"{bloomFilter}");
            filters++;
        }

        // Console.WriteLine(blo.ToString());

        RunQueries(blossom, addedWords);

        // var filter = filterList[0];
        // var filterPath = "./Assets/filter.json";

        // Console.WriteLine(filter.ToString());

        // BloomFilterFactory.ToPath(filter, filterPath);
        // var diskFilter = BloomFilterFactory.FromPath(filterPath, hashFunction);

        // Console.WriteLine(diskFilter.ToString());

        // foreach (var query in addedWords)
        // {
        //     if (query.Value == diskFilter.Id)
        //     {
        //         var contains = diskFilter.Contains(query.Key);
        //         Console.WriteLine($"Filter contains {query.Key}: {contains}");
        //     }
        // }
    }

    static void RunQueries(Blossom bloofi, Dictionary<string, Guid> addedWords)
    {
        int falseCount = 0;
        int trueCount = 0;
        foreach (var query in addedWords)
        {
            int count = bloofi.FindMatches(query.Key).Count;
            bool hasValue = count > 0;
            bool falsePos = count > 1;
            // Console.WriteLine($"Query: {query.Key} ({query.Value} ({count}))");
            if (falsePos)
            {
                // Console.WriteLine($"Bloofi does not contain {query.Key} ({query.Value})");
                falseCount++;
            }
            else
            {
                trueCount++;
            }
        }

        Console.WriteLine($"False count: {falseCount}");
        Console.WriteLine($"True count: {trueCount}");
    }
}