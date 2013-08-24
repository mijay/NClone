using System.Collections.Generic;
using System.Linq;
using NClone.Annotation;
using NClone.MemberCopying;
using NClone.Shared;

namespace NClone.TypeReplication
{
    /// <summary>
    /// Implementation of <see cref="IEntityReplicator{TType}"/> for value type <typeparamref name="TType"/>.
    /// </summary>
    internal class StructureReplicator<TType>: IEntityReplicator<TType>
    {
        private readonly IMetadataProvider metadataProvider;
        private readonly IMemberCopierBuilder memberCopierBuilder;

        public StructureReplicator(IMetadataProvider metadataProvider, IMemberCopierBuilder memberCopierBuilder)
        {
            Guard.Check(typeof (TType).IsValueType, "StructureReplicator can be applied only to value types");
            Guard.NotNull(metadataProvider, "metadataProvider");
            Guard.NotNull(memberCopierBuilder, "memberCopierBuilder");
            this.metadataProvider = metadataProvider;
            this.memberCopierBuilder = memberCopierBuilder;
        }

        public bool IsTrivial
        {
            get { return GetMemberReplicators().IsEmpty(); }
        }

        public TType Replicate(TType source)
        {
            var result = source;
            foreach (var memberCopier in GetMemberReplicators())
                result = memberCopier.Copy(source, result);
            return result;
        }

        private IEnumerable<IMemberCopier<TType>> GetMemberReplicators()
        {
            return metadataProvider
                .GetReplicatingMembers(typeof (TType))
                .Select(memberCopierBuilder.BuildFor<TType>)
                .Where(x => x.PerformsReplication);
            //todo: memorization
        }
    }
}