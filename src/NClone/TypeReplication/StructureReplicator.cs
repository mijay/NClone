using System;
using System.Collections.Generic;
using System.Linq;
using NClone.Annotation;
using NClone.FieldCopying;
using NClone.Shared;

namespace NClone.TypeReplication
{
    /// <summary>
    /// Implementation of <see cref="IEntityReplicator"/> for value type <see cref="EntityType"/>
    /// </summary>
    internal class StructureReplicator: IEntityReplicator
    {
        private readonly IMetadataProvider metadataProvider;
        private readonly IFieldCopiersBuilder fieldCopiersBuilder;

        public StructureReplicator(IMetadataProvider metadataProvider, IFieldCopiersBuilder fieldCopiersBuilder, Type entityType)
        {
            EntityType = entityType;
            Guard.AgainstViolation(entityType.IsValueType, "StructureReplicator can be applied only to value types");
            Guard.AgainstNull(metadataProvider, "metadataProvider");
            Guard.AgainstNull(fieldCopiersBuilder, "fieldCopiersBuilder");
            Guard.AgainstNull(entityType, "entityType");
            this.metadataProvider = metadataProvider;
            this.fieldCopiersBuilder = fieldCopiersBuilder;
        }

        public Type EntityType { get; private set; }

        public bool IsTrivial
        {
            get { return GetMemberReplicators().IsEmpty(); }
        }

        public object Replicate(object source)
        {
            Guard.AgainstViolation<InvalidCastException>(source.GetType() == EntityType);

            var result = source;
            foreach (var fieldCopier in GetMemberReplicators())
                result = fieldCopier.Copy(source, result);
            return result;
        }

        private IEnumerable<IFieldCopier> GetMemberReplicators()
        {
            return Memoized.Delegate(() =>
                metadataProvider
                    .GetReplicatingMembers(EntityType)
                    .Select(field => fieldCopiersBuilder.BuildFor(EntityType, field))
                    .Where(x => x.Replicating)
                    .ToArray());
        }
    }
}