using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NClone.MemberCopying;

namespace NClone.TypeReplication
{
    internal abstract class TypeReplicatorBase<TType>: ITypeReplicator<TType>
    {
        private readonly IMemberCopierBuilder memberCopierBuilder;

        protected TypeReplicatorBase(IMemberCopierBuilder memberCopierBuilder)
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

        public abstract bool IsRedundant { get; }
        public abstract TType Replicate(TType source);
    }
}