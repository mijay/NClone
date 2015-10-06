using System;
using System.Linq.Expressions;
using System.Reflection;
using BenchmarkDotNet;
using BenchmarkDotNet.Tasks;
using NClone.MemberAccess;

namespace NClone.Benchmarks.Competitions
{
    [BenchmarkTask(platform: BenchmarkPlatform.X86, framework: BenchmarkFramework.V35)]
    [BenchmarkTask(platform: BenchmarkPlatform.X86, framework: BenchmarkFramework.V40)]
    [BenchmarkTask(platform: BenchmarkPlatform.X86, framework: BenchmarkFramework.V45)]
    [BenchmarkTask(platform: BenchmarkPlatform.X64, framework: BenchmarkFramework.V40)]
    [BenchmarkTask(platform: BenchmarkPlatform.X64, framework: BenchmarkFramework.V45)]
    [BenchmarkTask(platform: BenchmarkPlatform.X64, framework: BenchmarkFramework.V451)]
    [BenchmarkTask(platform: BenchmarkPlatform.X64, framework: BenchmarkFramework.V452)]
    public class SetFieldCompetition
    {
        const int iterationCount = 200000;
        object someClass;
        Func<object, object, object> setViaExpression;
        Func<object, object, object> setViaEmit;
        Func<object, object, object> setViaReflection;

        [Setup]
        public void SetUp()
        {
            someClass = new SomeClass();
            var field = typeof (SomeClass).GetField("field", BindingFlags.NonPublic | BindingFlags.Instance);

            var xValue = Expression.Parameter(typeof (object));
            var xContainer = Expression.Parameter(typeof (object));
            var xTypedContainer = Expression.Parameter(typeof (SomeClass));
            var xSetField = Expression
                .Lambda<Func<object, object, object>>(
                    Expression.Block(new[] { xTypedContainer },
                        Expression.Assign(
                            xTypedContainer,
                            Expression.Convert(xContainer, typeof (SomeClass))),
                        Expression.Assign(
                            Expression.Field(xTypedContainer, field),
                            Expression.Convert(xValue, typeof (int))),
                        Expression.Convert(xTypedContainer, typeof (object))),
                    xContainer, xValue);
            setViaExpression = xSetField.Compile();

            var memberAccessor = FieldAccessorBuilder.BuildFor(typeof (SomeClass), field, true);
            setViaEmit = memberAccessor.SetMember;

            setViaReflection = (container, value) =>
            {
                field.SetValue(container, value);
                return container;
            };
        }

        [Benchmark, OperationsPerInvoke(iterationCount)]
        public void ViaExpression()
        {
            for (var i = 0; i < iterationCount; i++)
                someClass = setViaExpression(someClass, i);
        }

        [Benchmark, OperationsPerInvoke(iterationCount)]
        public void ViaEmit()
        {
            for (var i = 0; i < iterationCount; i++)
                someClass = setViaEmit(someClass, i);
        }

        [Benchmark, OperationsPerInvoke(iterationCount)]
        public void ViaReflection()
        {
            for (var i = 0; i < iterationCount; i++)
                someClass = setViaReflection(someClass, i);
        }

        class SomeClass
        {
            int field;
        }
    }
}
