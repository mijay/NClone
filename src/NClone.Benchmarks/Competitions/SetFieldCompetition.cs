using System;
using System.Linq.Expressions;
using System.Reflection;
using BenchmarkDotNet;
using NClone.MemberAccess;

namespace NClone.Benchmarks.Competitions
{
    public class SetFieldCompetition
    {
        private const int iterationCount = 200000;
        private object someClass;
        private Func<object, object, object> setViaExpression;
        private Func<object, object, object> setViaEmit;
        private Func<object, object, object> setViaReflection;

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

        private class SomeClass
        {
            private int field;
        }
    }
}