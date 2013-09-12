using System;
using System.Linq.Expressions;
using NClone.ObjectReplicators;
using NClone.Shared;

namespace NClone.SpecificTypeReplicators
{
    /// <summary>
    /// Implementation of <see cref="IReplicationStrategy"/> for nullable types.
    /// </summary>
    //note: while NonReplicatingStrategy used for all ValueType-s => there is no need to deep-copy Nullable-s
    //todo: no tests
    internal class NullableTypeReplicationStrategy: IReplicationStrategy
    {
        private readonly IObjectReplicator objectReplicator;
        private readonly Func<object, object> getUnderlyingValue;
        private readonly Func<object, object> buildNullable;

        public NullableTypeReplicationStrategy(IObjectReplicator objectReplicator, Type underlyingType)
        {
            Guard.AgainstNull(objectReplicator, "objectReplicator");
            Guard.AgainstNull(underlyingType, "underlyingType");
            Guard.AgainstViolation(underlyingType.IsValueType, "Underlying type should be value type");
            this.objectReplicator = objectReplicator;

            var nullableType = typeof (Nullable<>).MakeGenericType(underlyingType);
            getUnderlyingValue = BuildUnwrappingDelegate(nullableType);
            buildNullable = BuildWrappingDelegate(nullableType, underlyingType);
        }

        public object Replicate(object source)
        {
            Guard.AgainstNull(source, "source");

            var underlyingValue = getUnderlyingValue(source);
            var replicatedValue = objectReplicator.Replicate(underlyingValue);
            return replicatedValue != null ? buildNullable(replicatedValue) : null;
        }

        private static Func<object, object> BuildUnwrappingDelegate(Type nullableType)
        {
            var valueProperty = nullableType.GetProperty("Value");

            var xArgument = Expression.Parameter(typeof (object));
            return Expression
                .Lambda<Func<object, object>>(
                    Expression.Property(Expression.Convert(xArgument, nullableType), valueProperty),
                    xArgument)
                .Compile();
        }

        private static Func<object, object> BuildWrappingDelegate(Type nullableType, Type underlyingType)
        {
            var nullableTypeCtor = nullableType.GetConstructor(new[] { underlyingType });

            var xArgument = Expression.Parameter(typeof (object));
            var nullable = Expression
                .Lambda<Func<object, object>>(
                    Expression.New(nullableTypeCtor, Expression.Convert(xArgument, underlyingType)),
                    xArgument)
                .Compile();
            return nullable;
        }
    }
}