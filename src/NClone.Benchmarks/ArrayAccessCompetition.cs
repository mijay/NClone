using System;
using System.Collections;
using System.Linq;
using mijay.Utils;
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

        private static void Consume(Array array)
        {
            if (array.Length != arraySize)
                throw new Exception(array.Length.ToString());
        }

        [Benchmark]
        public Action ViaMethods()
        {
            Array source = CreateSource();

            return () => {
                       var destination = (Array) new int[arraySize];

                       for (int i = 0; i < arraySize; ++i) {
                           object data = source.GetValue(i);
                           destination.SetValue(data, i);
                       }

                       Consume(destination);
                   };
        }

        [Benchmark]
        public Action ViaEmit()
        {
            Array source = CreateSource();
            var accessor = ArrayAccessorBuilder.BuildForArrayOf(typeof (int));

            Func<Array, int, object> getMethod = accessor.GetElement;
            Action<Array, int, object> setMethod = accessor.SetElement;

            return () => {
                       var destination = (Array) new int[arraySize];

                       for (int i = 0; i < arraySize; ++i) {
                           object data = getMethod(source, i);
                           setMethod(destination, i, data);
                       }

                       Consume(destination);
                   };
        }

        [Benchmark]
        public Action ViaCast()
        {
            object[] source = CreateSource().Cast<int>().Cast<object>().ToArray();

            return () => {
                       var destination = new object[arraySize];

                       for (int i = 0; i < arraySize; ++i) {
                           object data = source[i];
                           destination[i] = data;
                       }

                       Consume(destination);
                   };
        }

        [Benchmark]
        public Action ViaEnumerable()
        {
            var source = CreateSource().As<IEnumerable>();

            return () => {
                       var destination = new ArrayList(arraySize);

                       foreach (var element in source)
                           destination.Add(element);

                       Consume(destination.ToArray(typeof (int)));
                   };
        }
    }
}