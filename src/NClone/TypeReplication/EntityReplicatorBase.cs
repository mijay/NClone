using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NClone.MemberCopying;

namespace NClone.TypeReplication
{
    internal abstract class EntityReplicatorBase<TType>: IEntityReplicator<TType>
    {
        private readonly IMemberCopierBuilder memberCopierBuilder;

        protected EntityReplicatorBase(IMemberCopierBuilder memberCopierBuilder)
        {
            this.memberCopierBuilder = memberCopierBuilder;
        }

        protected IEnumerable<IMemberCopier<TType>> GetMemberCopiers()
        {
            return typeof (TType)
                .GetFields(BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic)
                .Select(memberCopierBuilder.BuildFor<TType>);
            //todo: memorization
        }

        public abstract bool IsTrivial { get; }
        public abstract TType Replicate(TType source);
    }
}