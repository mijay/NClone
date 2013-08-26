using System;
using System.Linq;
using System.Runtime.Serialization;
using NClone.Annotation;
using NClone.FieldCopying;
using NClone.Shared;

namespace NClone.TypeReplication
{
    /// <summary>
    /// Implementation of <see cref="IEntityReplicator"/> for reference entityType <see cref="EntityType"/>.
    /// </summary>
    internal class ObjectReplicator: IEntityReplicator
    {
        private readonly IMetadataProvider metadataProvider;
        private readonly IFieldCopiersBuilder fieldCopiersBuilder;

        public ObjectReplicator(IMetadataProvider metadataProvider, IFieldCopiersBuilder fieldCopiersBuilder, Type entityType)
        {
            Guard.AgainstViolation(!entityType.IsValueType, "ObjectReplicator can be applied only to reference types");
            Guard.AgainstNull(metadataProvider, "metadataProvider");
            Guard.AgainstNull(fieldCopiersBuilder, "fieldCopiersBuilder");
            Guard.AgainstNull(entityType, "entityType");
            this.metadataProvider = metadataProvider;
            this.fieldCopiersBuilder = fieldCopiersBuilder;
            EntityType = entityType;
        }

        public Type EntityType { get; private set; }

        public bool IsTrivial
        {
            get { return false; }
        }

        public object Replicate(object source)
        {
            if (ReferenceEquals(source, null))
                return source;
            Guard.AgainstViolation<InvalidCastException>(source.GetType() == EntityType);

            var result = FormatterServices.GetUninitializedObject(EntityType);
            Memoized
                .Delegate(() => metadataProvider
                    .GetReplicatingMembers(EntityType)
                    .Select(field => fieldCopiersBuilder.BuildFor(EntityType, field))
                    .ToArray())
                .ForEach(fieldCopier => fieldCopier.Copy(source, result));
            return result;
        }
    }
}