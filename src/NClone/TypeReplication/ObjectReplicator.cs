using System.Runtime.Serialization;
using NClone.MemberCopying;
using NClone.Shared;

namespace NClone.TypeReplication
{
    /// <summary>
    /// Implementation of <see cref="IEntityReplicator{TType}"/> for reference type <typeparamref name="TType"/>.
    /// </summary>
    internal class ObjectReplicator<TType>: EntityReplicatorBase<TType>
    {
        public ObjectReplicator(IMemberCopierBuilder memberCopierBuilder): base(memberCopierBuilder)
        {
            Guard.Check(!typeof (TType).IsValueType, "ObjectReplicator can be applied only to reference types, but used for {0}", typeof (TType));
        }

        public override bool IsTrivial
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