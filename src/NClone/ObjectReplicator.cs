using System;
using System.Collections.Concurrent;
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
            //note: while DummyReplicator used for all ValueType-s => there is no need to deep-copy Nullable-s
            //if (type.IsNullable())
            //    return new NullableTypeReplicator(type.GetGenericArguments().Single());
            return type.IsValueType ? DummyReplicator.Instance : new ReferenceTypeReplicator(type);
        }
    }
}