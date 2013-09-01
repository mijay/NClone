using System;
using System.Collections.Concurrent;
using NClone.Annotation;
using NClone.Shared;
using NClone.SpecificTypeReplicators;

namespace NClone.ObjectReplicators
{
    /// <summary>
    /// Implementation of <see cref="IObjectReplicator"/>.
    /// </summary>
    public class ObjectReplicator: IObjectReplicator
    {
        private readonly IMetadataProvider metadataProvider;

        private readonly ConcurrentDictionary<Type, ISpecificTypeReplicator> entityReplicators =
            new ConcurrentDictionary<Type, ISpecificTypeReplicator>();

        public ObjectReplicator(IMetadataProvider metadataProvider)
        {
            Guard.AgainstNull(metadataProvider, "metadataProvider");
            this.metadataProvider = metadataProvider;
        }

        public object Replicate(object source)
        {
            if (ReferenceEquals(source, null))
                return null;
            var type = source.GetType();
            var entityReplicator = entityReplicators.GetOrAdd(type, BuildEntityReplicator);
            return entityReplicator.Replicate(source);
        }

        private ISpecificTypeReplicator BuildEntityReplicator(Type type)
        {
            if (type.IsPrimitive || type == typeof (string))
                return DummyReplicator.Instance;
            //note: while DummyReplicator used for all ValueType-s => there is no need to deep-copy Nullable-s
            //if (type.IsNullable())
            //    return new NullableTypeReplicator(type.GetGenericArguments().Single());
            return type.IsValueType ? DummyReplicator.Instance : new ReferenceTypeReplicator(metadataProvider, this, type);
        }
    }
}