using System.Collections.Generic;
using System.Linq;
using NClone.Annotation;
using NClone.MemberCopying;
using NClone.Shared;

namespace NClone.TypeReplication
{
    /// <summary>
    /// Implementation of <see cref="IEntityReplicator{TType}"/> for value type <typeparamref name="TEntity"/>.
    /// </summary>
    internal class StructureReplicator<TEntity>: IEntityReplicator<TEntity>
    {
        private readonly IMetadataProvider metadataProvider;
        private readonly IMemberCopierBuilder memberCopierBuilder;

        public StructureReplicator(IMetadataProvider metadataProvider, IMemberCopierBuilder memberCopierBuilder)
        {
            Guard.AgainstFalse(typeof (TEntity).IsValueType, "StructureReplicator can be applied only to value types");
            Guard.AgainstNull(metadataProvider, "metadataProvider");
            Guard.AgainstNull(memberCopierBuilder, "memberCopierBuilder");
            this.metadataProvider = metadataProvider;
            this.memberCopierBuilder = memberCopierBuilder;
        }

        public bool IsTrivial
        {
            get { return GetMemberReplicators().IsEmpty(); }
        }

        public TEntity Replicate(TEntity source)
        {
            var result = source;
            foreach (var memberCopier in GetMemberReplicators())
                result = memberCopier.Copy(source, result);
            return result;
        }

        private IEnumerable<IMemberCopier<TEntity>> GetMemberReplicators()
        {
            return metadataProvider
                .GetReplicatingMembers(typeof (TEntity))
                .Select(memberCopierBuilder.BuildFor<TEntity>)
                .Where(x => x.Replicate);
            //todo: memorization
        }
    }
}