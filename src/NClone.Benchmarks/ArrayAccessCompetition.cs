using System;
using System.Linq;
using NClone.Benchmarks.Runner;
using NClone.MemberAccess;

namespace NClone.Benchmarks
{
    public class ArrayAccessCompetition: CompetitionBase
    {
        private const int arraySize = 120000;

        private Array CreateSource()
        {
            var result = new int[arraySize];
            var random = new Random();
            for (int i = 0; i < arraySize; i++)
                result[i] = random.Next();
            return result;
        }

        private void ConsumeResult(Array result)
        {
            var array = (int[]) result;
            Console.WriteLine(array.Sum());
        }

        [Benchmark]
        public Action ViaMethods()
        {
            Array source = CreateSource();
            var destination = (Array) new int[arraySize];

            return () => {
                       for (int i = 0; i < arraySize; ++i) {
                           object data = source.GetValue(i);
                           destination.SetValue(data, i);
                       }
                   };
        }

        [Benchmark]
        public Action ViaEmit()
        {
            Array source = CreateSource();
            var destination = (Array) new int[arraySize];

            Func<Array, int, object> getMethod = ArrayAccessorBuilder.BuildArrayElementReader(typeof (int));
            Action<Array, int, object> setMethod = ArrayAccessorBuilder.BuildArrayElementWriter(typeof (int));

            return () => {
                       for (int i = 0; i < arraySize; ++i) {
                           object data = getMethod(source, i);
                           setMethod(destination, i, data);
                       }
                   };
        }
    }
}