using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using BenchmarkDotNet;

namespace NClone.Benchmarks.Competitions
{
    public class DictionaryCompetition
    {
        private const int itemsCount = 200000;
        private ConcurrentDictionary<int, string> concurrentDictionary;
        private Dictionary<int, string> dictionary;
        private ConcurrentDictionary<int, string> smallConcurrentDictionary;
        private Dictionary<int, string> smallDictionary;

        [Setup]
        public void Setup()
        {
            concurrentDictionary = new ConcurrentDictionary<int, string>();
            Populate(concurrentDictionary, itemsCount);
            dictionary = new Dictionary<int, string>();
            Populate(dictionary, itemsCount);
            smallConcurrentDictionary = new ConcurrentDictionary<int, string>();
            Populate(smallConcurrentDictionary, itemsCount/200);
            smallDictionary = new Dictionary<int, string>();
            Populate(smallDictionary, itemsCount/200);
        }

        [Benchmark, OperationsPerInvoke(itemsCount)]
        public void GetFromConcurrentDictionary()
        {
            var result = 0;
            for (var i = 0; i < itemsCount; i++)
                result += concurrentDictionary[i].Length;
            Consume(result);
        }

        [Benchmark, OperationsPerInvoke(itemsCount)]
        public void GetFromCommonDictionary()
        {
            var result = 0;
            for (var i = 0; i < itemsCount; i++)
                result += dictionary[i].Length;
            Consume(result);
        }

        [Benchmark, OperationsPerInvoke(itemsCount)]
        public void GetOrAddFromConcurrentDictionary()
        {
            var result = 0;
            for (var i = 0; i < itemsCount; i++)
                result += smallConcurrentDictionary.GetOrAdd(i, key => key.ToString()).Length;
            Consume(result);
        }

        [Benchmark, OperationsPerInvoke(itemsCount)]
        public void GetOrAddFromCommonDictionary()
        {
            var result = 0;
            for (var i = 0; i < itemsCount; i++)
            {
                string value;
                if (!smallDictionary.TryGetValue(i, out value))
                    smallDictionary.Add(i, value = i.ToString());
                result += value.Length;
            }
            Consume(result);
        }

        private static void Populate(IDictionary<int, string> dictionary, int count)
        {
            for (var i = 0; i < count; i++)
                dictionary[i] = i.ToString();
        }

        private static void Consume(int result)
        {
            if (result != 1088890)
                throw new Exception(result.ToString());
        }
    }
}