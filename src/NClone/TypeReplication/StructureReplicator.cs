using System.Collections.Generic;
using System.Linq;
using NClone.Annotation;
using NClone.FieldCopying;
using NClone.Shared;

namespace NClone.TypeReplication
{
    /// <summary>
    /// Implementation of <see cref="IEntityReplicator{TType}"/> for value type <typeparamref name="TEntity"/>.
    /// </summary>
    internal class StructureReplicator<TEntity>: IEntityReplicator<TEntity>
    {
        private readonly IMetadataProvider metadataProvider;
        private readonly IFieldCopiersBuilder fieldCopiersBuilder;

        public StructureReplicator(IMetadataProvider metadataProvider, IFieldCopiersBuilder fieldCopiersBuilder)
        {
            Guard.AgainstViolation(typeof (TEntity).IsValueType, "StructureReplicator can be applied only to value types");
            Guard.AgainstNull(metadataProvider, "metadataProvider");
            Guard.AgainstNull(fieldCopiersBuilder, "fieldCopiersBuilder");
            this.metadataProvider = metadataProvider;
            this.fieldCopiersBuilder = fieldCopiersBuilder;
        }

        public bool IsTrivial
        {
            get { return GetMemberReplicators().IsEmpty(); }
        }

        public TEntity Replicate(TEntity source)
        {
            var result = source;
            foreach (var fieldCopier in GetMemberReplicators())
                result = fieldCopier.Copy(source, result);
            return result;
        }

        private IEnumerable<IFieldCopier<TEntity>> GetMemberReplicators()
        {
            return metadataProvider
                .GetReplicatingMembers(typeof (TEntity))
                .Select(fieldCopiersBuilder.BuildFor<TEntity>)
                .Where(x => x.Replicating);
            //todo: memorization
        }
    }
}