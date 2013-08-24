using NClone.Annotation;
using NClone.MemberCopying;
using NClone.Shared;

namespace NClone.TypeReplication
{
    /// <summary>
    /// Builds <see cref="IEntityReplicator{TType}"/> for a given type.
    /// </summary>
    internal class EntityReplicatorBuilder
    {
        private readonly IMetadataProvider metadataProvider;
        private readonly IMemberCopierBuilder memberCopierBuilder;

        public EntityReplicatorBuilder(IMetadataProvider metadataProvider, IMemberCopierBuilder memberCopierBuilder)
        {
            Guard.AgainstNull(metadataProvider, "metadataProvider");
            Guard.AgainstNull(memberCopierBuilder, "memberCopierBuilder");
            this.memberCopierBuilder = memberCopierBuilder;
            this.metadataProvider = metadataProvider;
        }

        /// <summary>
        /// Builds <see cref="IEntityReplicator{TType}"/> for the given <typeparamref name="TType"/>.
        /// </summary>
        public IEntityReplicator<TType> BuildFor<TType>()
        {
            var type = typeof (TType);
            if (type == typeof (string) || type.IsEnum || type.IsPrimitive)
                return new TrivialReplicator<TType>();
            return type.IsValueType
                ? (IEntityReplicator<TType>) new StructureReplicator<TType>(metadataProvider, memberCopierBuilder)
                : new ObjectReplicator<TType>(metadataProvider, memberCopierBuilder);
            //todo: memorization
        }
    }
}