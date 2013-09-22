using System;
using System.Linq.Expressions;
using System.Reflection;
using NClone.ObjectReplicators;
using NClone.Shared;

namespace NClone.ReplicationStategies
{
    /// <summary>
    /// Implementation of <see cref="IReplicationStrategy"/> for nullable types.
    /// </summary>
    //todo: no tests
    internal class NullableTypeReplicationStrategy: IReplicationStrategy
    {
        private readonly Func<object, object> buildNullable;
        private readonly Func<object, object> getUnderlyingValue;
        private readonly IObjectReplicator objectReplicator;

        public NullableTypeReplicationStrategy(IObjectReplicator objectReplicator, Type nullableType)
        {
            Guard.AgainstNull(objectReplicator, "objectReplicator");
            Guard.AgainstNull(nullableType, "nullableType");
            Guard.AgainstViolation(nullableType.IsNullable(),
                "NullableTypeReplicationStrategy can work only with nullable types, but {0} received", nullableType);
            this.objectReplicator = objectReplicator;

            Type underlyingType = nullableType.GetNullableUnderlyingType();
            getUnderlyingValue = BuildUnwrappingDelegate(nullableType);
            buildNullable = BuildWrappingDelegate(nullableType, underlyingType);
        }

        public object Replicate(object source)
        {
            Guard.AgainstNull(source, "source");

            object underlyingValue = getUnderlyingValue(source);
            object replicatedValue = objectReplicator.Replicate(underlyingValue);
            return replicatedValue != null ? buildNullable(replicatedValue) : null;
        }

        private static Func<object, object> BuildUnwrappingDelegate(Type nullableType)
        {
            PropertyInfo valueProperty = nullableType.GetProperty("Value");

            ParameterExpression xArgument = Expression.Parameter(typeof (object));
            return Expression
                .Lambda<Func<object, object>>(
                    Expression.Property(Expression.Convert(xArgument, nullableType), valueProperty),
                    xArgument)
                .Compile();
        }

        private static Func<object, object> BuildWrappingDelegate(Type nullableType, Type underlyingType)
        {
            ConstructorInfo nullableTypeCtor = nullableType.GetConstructor(new[] { underlyingType });

            ParameterExpression xArgument = Expression.Parameter(typeof (object));
            return Expression
                .Lambda<Func<object, object>>(
                    Expression.New(nullableTypeCtor, Expression.Convert(xArgument, underlyingType)),
                    xArgument)
                .Compile();
        }
    }
}