using NClone.Annotation;
using NClone.MemberCopying;
using NClone.Shared;

namespace NClone.TypeReplication
{
    internal class TypeReplicatorBuilder
    {
        private readonly IMetadataProvider metadataProvider;
        private readonly IMemberCopierBuilder memberCopierBuilder;

        public TypeReplicatorBuilder(IMetadataProvider metadataProvider, IMemberCopierBuilder memberCopierBuilder)
        {
            Guard.NotNull(metadataProvider, "metadataProvider");
            Guard.NotNull(memberCopierBuilder, "memberCopierBuilder");
            this.memberCopierBuilder = memberCopierBuilder;
            this.metadataProvider = metadataProvider;
        }

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