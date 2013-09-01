using System;
using System.Collections.Concurrent;
using System.Linq;
using NClone.EntityReplicators;
using NClone.Shared;

namespace NClone
{
    public static class ObjectReplicator
    {
        private static readonly ConcurrentDictionary<Type, IEntityReplicator> entityReplicators =
            new ConcurrentDictionary<Type, IEntityReplicator>();

        public static T Replicate<T>(T source)
        {
            return Replicate(source.As<object>()).As<T>();
        }

        internal static object Replicate(object source)
        {
            if (ReferenceEquals(source, null))
                return null;
            var type = source.GetType();
            var entityReplicator = entityReplicators.GetOrAdd(type, BuildEntityReplicator);
            return entityReplicator.Replicate(source);
        }

        private static IEntityReplicator BuildEntityReplicator(Type type)
        {
            if (type.IsPrimitive || type == typeof (string))
                return DummyReplicator.Instance;
            if (type.IsNullable())
                return new NullableTypeReplicator(type.GetGenericArguments().Single());
            if (type.IsValueType)
                return DummyReplicator.Instance;
            return new ReferenceTypeReplicator(type);
        }
    }
}