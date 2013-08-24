using System.Linq;
using System.Runtime.Serialization;
using NClone.Annotation;
using NClone.MemberCopying;
using NClone.Shared;

namespace NClone.TypeReplication
{
    /// <summary>
    /// Implementation of <see cref="IEntityReplicator{TType}"/> for reference type <typeparamref name="TType"/>.
    /// </summary>
    internal class ObjectReplicator<TType>: IEntityReplicator<TType>
    {
        private readonly IMetadataProvider metadataProvider;
        private readonly IMemberCopierBuilder memberCopierBuilder;

        public ObjectReplicator(IMetadataProvider metadataProvider, IMemberCopierBuilder memberCopierBuilder)
        {
            Guard.Check(!typeof (TType).IsValueType, "ObjectReplicator can be applied only to reference types");
            Guard.NotNull(metadataProvider, "metadataProvider");
            Guard.NotNull(memberCopierBuilder, "memberCopierBuilder");
            this.metadataProvider = metadataProvider;
            this.memberCopierBuilder = memberCopierBuilder;
        }

        public bool IsTrivial
        {
            get { return false; }
        }

        public TType Replicate(TType source)
        {
            var result = (TType) FormatterServices.GetUninitializedObject(typeof (TType));
            metadataProvider
                .GetReplicatingMembers(typeof (TType))
                .Select(memberCopierBuilder.BuildFor<TType>)
                // todo: memorization
                .ForEach(memberCopier => memberCopier.Copy(source, result));
            return result;
        }
    }
}