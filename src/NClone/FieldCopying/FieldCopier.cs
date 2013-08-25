using System.Reflection;
using JetBrains.Annotations;
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
        private static readonly MethodInfo typedReplicateMethod = typeof (FieldCopier<TEntity, TMember>)
            .GetMethod("Replicate", BindingFlags.Instance | BindingFlags.NonPublic);

        private readonly IEntityReplicatorsBuilder entityReplicatorBuilder;
        private readonly IMemberAccessor<TEntity, TMember> memberAccessor;
        private IEntityReplicator<TMember> defaultReplicator;

        public FieldCopier(IEntityReplicatorsBuilder entityReplicatorBuilder, IMemberAccessor<TEntity, TMember> memberAccessor)
        {
            Guard.AgainstNull(memberAccessor, "memberAccessor");
            Guard.AgainstNull(entityReplicatorBuilder, "entityReplicatorBuilder");
            this.entityReplicatorBuilder = entityReplicatorBuilder;
            this.memberAccessor = memberAccessor;
            defaultReplicator = entityReplicatorBuilder.BuildFor<TMember>();
            Replicating = !defaultReplicator.IsTrivial;
        }

        public TEntity Copy(TEntity source, TEntity destination)
        {
            var value = memberAccessor.GetMember(source);
            if (value != null && Replicating) {
                var actualType = value.GetType();
                value = actualType == typeof (TMember)
                    ? defaultReplicator.Replicate(value)
                    : typedReplicateMethod.MakeGenericMethod(actualType).Invoke(this, new object[] { value }).As<TMember>();
            }
            return memberAccessor.SetMember(destination, value);
        }

        public bool Replicating { get; private set; }

        [UsedImplicitly]
        private TMember Replicate<TActualMemberType>(TActualMemberType value)
            where TActualMemberType: TMember
        {
            return entityReplicatorBuilder.BuildFor<TActualMemberType>().Replicate(value);
        }
    }
}