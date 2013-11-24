using System;
using System.Linq.Expressions;
using System.Reflection;
using NClone.Benchmarks.Runner;
using NClone.MemberAccess;

namespace NClone.Benchmarks
{
    public class SetFieldCompetition: CompetitionBase
    {
        private const int iterationCount = 200000;

        [Benchmark]
        public Action ViaExpression()
        {
            FieldInfo field = typeof (SomeClass).GetField("field", BindingFlags.NonPublic | BindingFlags.Instance);

            ParameterExpression xValue = Expression.Parameter(typeof (object));
            ParameterExpression xContainer = Expression.Parameter(typeof (object));
            ParameterExpression xTypedContainer = Expression.Parameter(typeof (SomeClass));
            Expression<Func<object, object, object>> xSetField = Expression
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

            object someClass = new SomeClass();
            Func<object, object, object> setFieldDelegate = xSetField.Compile();

            return () => {
                       for (int i = 0; i < iterationCount; i++)
                           someClass = setFieldDelegate(someClass, i);
                   };
        }

        [Benchmark]
        public Action ViaEmit()
        {
            FieldInfo field = typeof (SomeClass).GetField("field", BindingFlags.NonPublic | BindingFlags.Instance);
            IMemberAccessor memberAccessor = FieldAccessorBuilder.BuildFor(typeof (SomeClass), field, true);

            object someClass = new SomeClass();
            var setMemberDelegate = new Func<object, object, object>(memberAccessor.SetMember);

            return () => {
                       for (int i = 0; i < iterationCount; i++)
                           someClass = setMemberDelegate(someClass, i);
                   };
        }

        [Benchmark]
        public Action ViaReflection()
        {
            FieldInfo field = typeof (SomeClass).GetField("field", BindingFlags.NonPublic | BindingFlags.Instance);

            object someClass = new SomeClass();
            Func<object, object, object> setMemberDelegate = (container, value) => {
                                                                 field.SetValue(container, value);
                                                                 return container;
                                                             };

            return () => {
                       for (int i = 0; i < iterationCount; i++)
                           someClass = setMemberDelegate(someClass, i);
                   };
        }

        private class SomeClass
        {
            private int field;
        }
    }
}