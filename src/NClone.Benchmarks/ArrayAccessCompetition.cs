using System;
using System.Linq;
using System.Reflection.Emit;
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

            var getMethodDeclaration = new DynamicMethod(
                "getFromArray",
                typeof (object), new[] { typeof (Array), typeof (int) },
                GetType(), true);
            ILGenerator getMethodGenerator = getMethodDeclaration.GetILGenerator();

            getMethodGenerator
                .LoadArgument(0)
                .CastDownPointer(typeof (int[]))
                .LoadArgument(1)
                .LoadArrayElement(typeof (int))
                .BoxValue(typeof (int))
                .Return();

            var getMethod = (Func<Array, int, object>) getMethodDeclaration.CreateDelegate(typeof (Func<Array, int, object>));

            var setMethodDeclaration = new DynamicMethod(
                "getFromArray",
                null, new[] { typeof (Array), typeof (int), typeof (object) },
                GetType(), true);
            ILGenerator setMethodGenerator = setMethodDeclaration.GetILGenerator();

            setMethodGenerator
                .LoadArgument(0)
                .CastDownPointer(typeof (int[]))
                .LoadArgument(1)
                .LoadArgument(2)
                .CastDownPointer(typeof (int))
                .StoreArrayElement(typeof (int))
                .Return();

            var setMethod = (Action<Array, int, object>) setMethodDeclaration.CreateDelegate(typeof (Action<Array, int, object>));

            return () => {
                       for (int i = 0; i < arraySize; ++i) {
                           object data = getMethod(source, i);
                           setMethod(destination, i, data);
                       }
                   };
        }
    }
}