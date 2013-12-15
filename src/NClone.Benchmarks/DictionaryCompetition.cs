using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using NClone.Benchmarks.Runner;

namespace NClone.Benchmarks
{
    public class DictionaryCompetition: CompetitionBase
    {
        private const int iterationsCount = 200000;

        private static void Populate(IDictionary<int, string> dictionary, int count)
        {
            for (int i = 0; i < count; i++)
                dictionary[i] = i.ToString();
        }

        private static void Consume(int result)
        {
            if (result != 1088890)
                throw new Exception(result.ToString());
        }

        [Benchmark]
        public Action GetFromConcurrentDictionary()
        {
            var dictionary = new ConcurrentDictionary<int, string>();
            Populate(dictionary, iterationsCount);

            return () => {
                       int result = 0;
                       for (int i = 0; i < iterationsCount; i++)
                           result += dictionary[i].Length;
                       Consume(result);
                   };
        }

        [Benchmark]
        public Action GetFromCommonDictionary()
        {
            var dictionary = new Dictionary<int, string>();
            Populate(dictionary, iterationsCount);

            return () => {
                       int result = 0;
                       for (int i = 0; i < iterationsCount; i++)
                           result += dictionary[i].Length;
                       Consume(result);
                   };
        }

        [Benchmark]
        public Action GetOrAddFromConcurrentDictionary()
        {
            var dictionary = new ConcurrentDictionary<int, string>();
            Populate(dictionary, iterationsCount / 200);

            return () => {
                       int result = 0;
                       for (int i = 0; i < iterationsCount; i++)
                           result += dictionary.GetOrAdd(i, key => key.ToString()).Length;
                       Consume(result);
                   };
        }

        [Benchmark]
        public Action GetOrAddFromCommonDictionary()
        {
            var dictionary = new Dictionary<int, string>();
            Populate(dictionary, iterationsCount / 200);

            return () => {
                       int result = 0;
                       for (int i = 0; i < iterationsCount; i++) {
                           string value;
                           if (!dictionary.TryGetValue(i, out value)) {
                               dictionary.Add(i, value = i.ToString());
                           }
                           result += value.Length;
                       }
                       Consume(result);
                   };
        }
    }
}