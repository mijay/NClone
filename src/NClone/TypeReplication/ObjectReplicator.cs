using System.Runtime.Serialization;
using NClone.MemberCopying;

namespace NClone.TypeReplication
{
    internal class ObjectReplicator<TType>: TypeReplicatorBase<TType>
    {
        public ObjectReplicator(IMemberCopierBuilder memberCopierBuilder): base(memberCopierBuilder)
        {
        }

        public override bool IsRedundant
        {
            get { return false; }
        }

        public override TType Replicate(TType source)
        {
            var result = (TType) FormatterServices.GetUninitializedObject(typeof (TType));
            foreach (var memberCopier in GetMemberCopiers())
                memberCopier.Copy(source, result);
            return result;
        }
    }
}