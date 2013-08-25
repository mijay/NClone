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

        public IEntityReplicator<TType> BuildFor<TType>()
        {
            return Memoized.Delegate(delegate {
                var type = typeof (TType);
                if (type == typeof (string) || type.IsEnum || type.IsPrimitive)
                    return new TrivialReplicator<TType>();
                return type.IsValueType
                    ? (IEntityReplicator<TType>) new StructureReplicator<TType>(metadataProvider, fieldCopiersBuilder)
                    : new ObjectReplicator<TType>(metadataProvider, fieldCopiersBuilder);
            });
        }
    }
}