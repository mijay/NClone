using System;
using System.Reflection;
using NClone.MemberAccess;
using NClone.Shared;
using NClone.TypeReplication;

namespace NClone.FieldCopying
{
    /// <summary>
    /// Implementation of <see cref="IFieldCopier"/>
    /// </summary>
    internal class FieldCopier: IFieldCopier
    {
        private readonly IEntityReplicatorsBuilder entityReplicatorsBuilder;
        private readonly Type containerType;
        private readonly FieldInfo field;
        private IMemberAccessor memberAccessor;

        public FieldCopier(IEntityReplicatorsBuilder entityReplicatorsBuilder, Type containerType, FieldInfo field)
        {
            Guard.AgainstNull(entityReplicatorsBuilder, "entityReplicatorsBuilder");
            Guard.AgainstNull(containerType, "containerType");
            Guard.AgainstNull(field, "field");
            this.entityReplicatorsBuilder = entityReplicatorsBuilder;
            this.containerType = containerType;
            this.field = field;
            Replicating = !entityReplicatorsBuilder.BuildFor(field.FieldType).IsTrivial;
        }

        public object Copy(object source, object destination)
        {
            if (memberAccessor == null)
                memberAccessor = FieldAccessorBuilder.BuildFor(containerType, field);

            var value = memberAccessor.GetMember(source);
            if (value != null && Replicating) {
                var actualType = value.GetType();
                value = entityReplicatorsBuilder.BuildFor(actualType).Replicate(value);
            }
            return memberAccessor.SetMember(destination, value);
        }

        public bool Replicating { get; private set; }
    }
}