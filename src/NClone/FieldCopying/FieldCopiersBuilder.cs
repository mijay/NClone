using System;
using System.Reflection;
using JetBrains.Annotations;
using NClone.MemberAccess;
using NClone.Shared;
using NClone.TypeReplication;

namespace NClone.FieldCopying
{
    /// <summary>
    /// Implementation of <see cref="IFieldCopiersBuilder"/>.
    /// </summary>
    internal class FieldCopiersBuilder: IFieldCopiersBuilder
    {
        private readonly Lazy<IEntityReplicatorsBuilder> entityReplicatorBuilder;

        private static readonly MethodInfo typedBuilder =
            typeof (FieldCopiersBuilder).GetMethod("BuildFor", BindingFlags.NonPublic | BindingFlags.Instance);

        public FieldCopiersBuilder(Lazy<IEntityReplicatorsBuilder> entityReplicatorBuilder)
        {
            Guard.AgainstNull(entityReplicatorBuilder, "entityReplicatorBuilder");
            this.entityReplicatorBuilder = entityReplicatorBuilder;
        }

        public IFieldCopier<TEntity> BuildFor<TEntity>(FieldInfo field)
        {
            Guard.AgainstNull(field, "field");
            Guard.AgainstViolation(field.DeclaringType.IsAssignableFrom(typeof (TEntity)),
                "Only fields of {0} can be copied", typeof (TEntity));

            return typedBuilder
                .MakeGenericMethod(typeof (TEntity), field.FieldType)
                .Invoke(this, new object[] { field })
                .As<IFieldCopier<TEntity>>();
        }

        [UsedImplicitly]
        private IFieldCopier<TEntity> BuildFor<TEntity, TMember>(FieldInfo field)
        {
            var memberAccessor = FieldAccessorBuilder.BuildFor<TEntity, TMember>(field, true);
            return new FieldCopier<TEntity, TMember>(entityReplicatorBuilder.Value, memberAccessor);
        }
    }
}