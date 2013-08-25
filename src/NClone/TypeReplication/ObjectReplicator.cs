using System;
using System.Linq;
using System.Runtime.Serialization;
using NClone.Annotation;
using NClone.FieldCopying;
using NClone.Shared;

namespace NClone.TypeReplication
{
    /// <summary>
    /// Implementation of <see cref="IEntityReplicator{TType}"/> for reference type <typeparamref name="TType"/>.
    /// </summary>
    internal class ObjectReplicator<TType>: IEntityReplicator<TType>
    {
        private readonly IMetadataProvider metadataProvider;
        private readonly IFieldCopiersBuilder fieldCopiersBuilder;

        public ObjectReplicator(IMetadataProvider metadataProvider, IFieldCopiersBuilder fieldCopiersBuilder)
        {
            Guard.AgainstViolation(!typeof (TType).IsValueType, "ObjectReplicator can be applied only to reference types");
            Guard.AgainstNull(metadataProvider, "metadataProvider");
            Guard.AgainstNull(fieldCopiersBuilder, "fieldCopiersBuilder");
            this.metadataProvider = metadataProvider;
            this.fieldCopiersBuilder = fieldCopiersBuilder;
        }

        public bool IsTrivial
        {
            get { return false; }
        }

        public TType Replicate(TType source)
        {
            if (ReferenceEquals(source, null))
                return source;
            Guard.AgainstViolation<InvalidCastException>(source.GetType() == typeof (TType));

            var result = (TType) FormatterServices.GetUninitializedObject(typeof (TType));
            metadataProvider
                .GetReplicatingMembers(typeof (TType))
                .Select(fieldCopiersBuilder.BuildFor<TType>)
                // todo: memorization
                .ForEach(memberCopier => memberCopier.Copy(source, result));
            return result;
        }
    }
}