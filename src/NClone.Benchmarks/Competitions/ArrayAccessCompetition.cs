using System;
using System.Collections;
using System.Linq;
using BenchmarkDotNet;
using NClone.MemberAccess;

namespace NClone.Benchmarks.Competitions
{
    public class ArrayAccessCompetition
    {
        private const int arraySize = 120000;

        private Array sourceArray;
        private Func<Array, int, object> getByAccessor;
        private Action<Array, int, object> setByAccessor;

        [Setup]
        public void SetUp()
        {
            var source = new int[arraySize];
            var random = new Random();
            for (var i = 0; i < arraySize; i++)
                source[i] = random.Next();

            sourceArray = source;

            var accessor = ArrayAccessorBuilder.BuildForArrayOf(typeof (int));
            getByAccessor = accessor.GetElement;
            setByAccessor = accessor.SetElement;
        }

        [Benchmark, OperationsPerInvoke(arraySize)]
        public void ViaMethods()
        {
            var destination = (Array) new int[arraySize];

            for (var i = 0; i < arraySize; ++i)
            {
                var data = sourceArray.GetValue(i);
                destination.SetValue(data, i);
            }

            Consume(destination);
        }

        [Benchmark, OperationsPerInvoke(arraySize)]
        public void ViaEmit()
        {
            var destination = (Array) new int[arraySize];

            for (var i = 0; i < arraySize; ++i)
            {
                var data = getByAccessor(sourceArray, i);
                setByAccessor(destination, i, data);
            }

            Consume(destination);
        }

        [Benchmark, OperationsPerInvoke(arraySize)]
        public void ViaEnumerable()
        {
            var destination = new ArrayList(arraySize);

            foreach (var element in sourceArray)
                destination.Add(element);

            Consume(destination.ToArray(typeof (int)));
        }

        private static void Consume(Array array)
        {
            if (array.Length != arraySize)
                throw new Exception(array.Length.ToString());
        }
    }
}