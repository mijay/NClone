using NClone.MemberAccess;
using NClone.Shared;
using NClone.TypeReplication;

namespace NClone.FieldCopying
{
    /// <summary>
    /// Implementation of <see cref="IFieldCopier{TEntity}"/>
    /// </summary>
    internal class FieldCopier<TEntity, TMember>: IFieldCopier<TEntity>
    {
        private readonly IEntityReplicatorBuilder entityReplicatorBuilder;
        private readonly IMemberAccessor<TEntity, TMember> memberAccessor;

        public FieldCopier(IEntityReplicatorBuilder entityReplicatorBuilder, IMemberAccessor<TEntity, TMember> memberAccessor)
        {
            Guard.AgainstNull(memberAccessor, "memberAccessor");
            Guard.AgainstNull(entityReplicatorBuilder, "entityReplicatorBuilder");
            this.entityReplicatorBuilder = entityReplicatorBuilder;
            this.memberAccessor = memberAccessor;
            Replicating = !entityReplicatorBuilder.BuildFor<TMember>().IsTrivial;
        }

        public TEntity Copy(TEntity source, TEntity destination)
        {
            var value = memberAccessor.GetMember(source);
            //if (value != null && Replicating) //todo: test for null
            //{
            //    var entityReplicator = entityReplicatorBuilder.BuildFor<TMember>(value.GetType());
            //    if (!entityReplicator.IsTrivial)
            //        value = entityReplicator.Replicate(value);
            //}
            return memberAccessor.SetMember(destination, value);
        }

        public bool Replicating { get; private set; }
    }
}