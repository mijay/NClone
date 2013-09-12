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

        private readonly ConcurrentDictionary<Type, IReplicationStrategy> entityReplicators =
            new ConcurrentDictionary<Type, IReplicationStrategy>();

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

        private IReplicationStrategy BuildEntityReplicator(Type type)
        {
            if (type.IsPrimitive || type == typeof (string))
                return NonReplicatingStrategy.Instance;
            //note: while NonReplicatingStrategy used for all ValueType-s => there is no need to deep-copy Nullable-s
            //if (type.IsNullable())
            //    return new NullableTypeReplicator(type.GetGenericArguments().Single());
            return type.IsValueType ? NonReplicatingStrategy.Instance : new CommonReplicationStrategy(metadataProvider, this, type);
        }
    }
}