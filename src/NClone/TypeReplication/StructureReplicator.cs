using System.Linq;
using NClone.MemberCopying;

namespace NClone.TypeReplication
{
    internal class StructureReplicator<TType>: TypeReplicatorBase<TType>
    {
        public StructureReplicator(IMemberCopierBuilder memberCopierBuilder): base(memberCopierBuilder)
        {
        }

        public override bool IsRedundant
        {
            get { return GetMemberCopiers().All(x => x.IsTrivial); }
        }

        public override TType Replicate(TType source)
        {
            var result = source;
            foreach (var memberCopier in GetMemberCopiers().Where(x => !x.IsTrivial))
                result = memberCopier.Copy(source, result);
            return result;
        }
    }
}