using System.Linq;
using NClone.MemberCopying;
using NClone.Shared;

namespace NClone.TypeReplication
{
    /// <summary>
    /// Implementation of <see cref="IEntityReplicator{TType}"/> for value type <typeparamref name="TType"/>.
    /// </summary>
    internal class StructureReplicator<TType>: EntityReplicatorBase<TType>
    {
        public StructureReplicator(IMemberCopierBuilder memberCopierBuilder): base(memberCopierBuilder)
        {
            Guard.Check(typeof (TType).IsValueType, "StructureReplicator can be applied only to value types, but used for {0}", typeof (TType));
        }

        public override bool IsTrivial
        {
            get { return GetMemberCopiers().All(x => !x.PerformsReplication); }
        }

        public override TType Replicate(TType source)
        {
            var result = source;
            foreach (var memberCopier in GetMemberCopiers().Where(x => x.PerformsReplication))
                result = memberCopier.Copy(source, result);
            return result;
        }
    }
}