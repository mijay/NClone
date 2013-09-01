using System;
using System.Linq.Expressions;
using NClone.Shared;

namespace NClone.SecondVersion
{
    /// <summary>
    /// Implementation of <see cref="IEntityReplicator"/> for nullable types.
    /// </summary>
    internal class NullableTypeReplicator: IEntityReplicator
    {
        private readonly Func<object, object> getUnderlyingValue;
        private readonly Func<object, object> buildNullable;

        public NullableTypeReplicator(Type underlyingType)
        {
            Guard.AgainstNull(underlyingType, "underlyingType");
            Guard.AgainstViolation(underlyingType.IsValueType, "Underlying type should be value type");

            var nullableType = typeof (Nullable<>).MakeGenericType(underlyingType);
            var nullableTypeCtor = nullableType.GetConstructor(new[] { underlyingType });
            var valueProperty = nullableType.GetProperty("Value");

            var xArgument = Expression.Parameter(typeof (object));
            getUnderlyingValue = Expression
                .Lambda<Func<object, object>>(
                    Expression.Property(Expression.Convert(xArgument, nullableType), valueProperty),
                    xArgument)
                .Compile();

            buildNullable = Expression
                .Lambda<Func<object, object>>(
                    Expression.New(nullableTypeCtor, Expression.Convert(xArgument, underlyingType)),
                    xArgument)
                .Compile();
        }

        public object Replicate(object source)
        {
            var underlyingValue = getUnderlyingValue(source);
            var replicatedValue = ObjectReplicator.Replicate(underlyingValue);
            return replicatedValue != null ? buildNullable(replicatedValue) : null;
        }
    }
}