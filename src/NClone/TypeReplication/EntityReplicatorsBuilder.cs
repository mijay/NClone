using System;
using NClone.Annotation;
using NClone.FieldCopying;
using NClone.Shared;

namespace NClone.TypeReplication
{
    /// <summary>
    /// Implementation of <see cref="IEntityReplicatorsBuilder"/>.
    /// </summary>
    internal class EntityReplicatorsBuilder: IEntityReplicatorsBuilder
    {
        private readonly IMetadataProvider metadataProvider;
        private readonly IFieldCopiersBuilder fieldCopiersBuilder;

        public EntityReplicatorsBuilder(IMetadataProvider metadataProvider, IFieldCopiersBuilder fieldCopiersBuilder)
        {
            Guard.AgainstNull(metadataProvider, "metadataProvider");
            Guard.AgainstNull(fieldCopiersBuilder, "fieldCopiersBuilder");
            this.fieldCopiersBuilder = fieldCopiersBuilder;
            this.metadataProvider = metadataProvider;
        }

        public IEntityReplicator BuildFor(Type entityType)
        {
            return Memoized.Delegate(delegate(Type type) {
                if (type == typeof (string) || type.IsEnum || type.IsPrimitive)
                    return new TrivialReplicator(type);
                return type.IsValueType
                    ? (IEntityReplicator) new StructureReplicator(metadataProvider, fieldCopiersBuilder, type)
                    : new ObjectReplicator(metadataProvider, fieldCopiersBuilder, type);
            }, entityType);
        }
    }
}