using System;
using System.Collections;
using System.Linq.Expressions;
using BenchmarkDotNet;
using BenchmarkDotNet.Tasks;
using NClone.MemberAccess;

namespace NClone.Benchmarks.Competitions
{
    [BenchmarkTask(platform: BenchmarkPlatform.X86, framework: BenchmarkFramework.V35, jitVersion: BenchmarkJitVersion.LegacyJit)]
    [BenchmarkTask(platform: BenchmarkPlatform.X86, framework: BenchmarkFramework.V40, jitVersion: BenchmarkJitVersion.LegacyJit)]
    [BenchmarkTask(platform: BenchmarkPlatform.X86, framework: BenchmarkFramework.V45, jitVersion: BenchmarkJitVersion.LegacyJit)]
    [BenchmarkTask(platform: BenchmarkPlatform.X86, framework: BenchmarkFramework.V46, jitVersion: BenchmarkJitVersion.RyuJit)]
    [BenchmarkTask(platform: BenchmarkPlatform.X64, framework: BenchmarkFramework.V40, jitVersion: BenchmarkJitVersion.LegacyJit)]
    [BenchmarkTask(platform: BenchmarkPlatform.X64, framework: BenchmarkFramework.V45, jitVersion: BenchmarkJitVersion.LegacyJit)]
    [BenchmarkTask(platform: BenchmarkPlatform.X64, framework: BenchmarkFramework.V46, jitVersion: BenchmarkJitVersion.RyuJit)]
    public class ArrayAccessCompetition
    {
        const int arraySize = 120000;

        Array sourceArray;
        Func<Array, int, object> getByAccessor;
        Action<Array, int, object> setByAccessor;
        Action<Array, int, Array> setViaExpression;

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

            var xSourceArray = Expression.Parameter(typeof (Array));
            var xDestinationArray = Expression.Parameter(typeof (Array));
            var xIndex = Expression.Parameter(typeof (int));
            var xSetItem = Expression.Lambda<Action<Array, int, Array>>(
                Expression.Assign(
                    Expression.ArrayAccess(Expression.Convert(xDestinationArray, typeof (int[])), xIndex),
                    Expression.ArrayAccess(Expression.Convert(xSourceArray, typeof (int[])), xIndex)
                    ),
                xDestinationArray, xIndex, xSourceArray);
            setViaExpression = xSetItem.Compile();
        }

        [Benchmark, OperationsPerInvoke(arraySize)]
        public void ViaExpression()
        {
            var destination = (Array) new int[arraySize];

            for (var i = 0; i < arraySize; ++i)
                setViaExpression(destination, i, sourceArray);

            Consume(destination);
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

        static void Consume(Array array)
        {
            if (array.Length != arraySize)
                throw new Exception(array.Length.ToString());
        }
    }
}
