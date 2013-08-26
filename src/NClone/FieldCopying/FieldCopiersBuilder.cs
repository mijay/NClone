using System;
using System.Reflection;
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

        public FieldCopiersBuilder(Lazy<IEntityReplicatorsBuilder> entityReplicatorBuilder)
        {
            Guard.AgainstNull(entityReplicatorBuilder, "entityReplicatorBuilder");
            this.entityReplicatorBuilder = entityReplicatorBuilder;
        }

        public IFieldCopier BuildFor(Type container, FieldInfo field)
        {
            Guard.AgainstNull(field, "field");
            Guard.AgainstViolation(field.DeclaringType.IsAssignableFrom(container),
                "Only fields of {0} can be copied", container);

            return new FieldCopier(entityReplicatorBuilder.Value, container, field);
        }
    }
}