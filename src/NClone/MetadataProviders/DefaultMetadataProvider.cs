using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NClone.Shared;

namespace NClone.MetadataProviders
{
    /// <summary>
    /// Implementation of <see cref="IMetadataProvider"/> that provides basic and always applicable functionality.
    /// </summary>
    /// <remarks>
    /// <para><see cref="DefaultMetadataProvider"/> defines that:
    /// <list type="number">
    /// <item>Primitive types, enums and <c>string</c> should be just <see cref="ReplicationBehavior.Copy"/>.</item>
    /// <item><see cref="Nullable{T}"/> types inherit <see cref="ReplicationBehavior"/> from underlying type.</item>
    /// <item>When type is replicated, all its fields should be replicated.</item>
    /// </list>
    /// </para>
    /// </remarks>
    public class DefaultMetadataProvider: IMetadataProvider
    {
        public virtual ReplicationBehavior GetPerTypeBehavior(Type type)
        {
            ReplicationBehavior behavior;
            if (TryGetDefaultBehavior(type, out behavior))
                return behavior;
            return ReplicationBehavior.Replicate;
        }

        public virtual IEnumerable<FieldReplicationInfo> GetFieldsReplicationInfo(Type type)
        {
            return GetAllFields(type).Select(x => new FieldReplicationInfo(x.BackingField, ReplicationBehavior.Replicate));
        }

        protected bool TryGetDefaultBehavior(Type entityType, out ReplicationBehavior behavior)
        {
            Guard.AgainstNull(entityType, "entityType");

            if (entityType.IsPrimitive || entityType.IsEnum || entityType == typeof (string)) {
                behavior = ReplicationBehavior.Copy;
                return true;
            }
            if (entityType.IsNullable()) {
                Type underlyingType = entityType.GetNullableUnderlyingType();
                behavior = GetPerTypeBehavior(underlyingType);
                return true;
            }
            behavior = ReplicationBehavior.Replicate;
            return false;
        }

        protected static IEnumerable<CopyableFieldDescription> GetAllFields(Type entityType)
        {
            Guard.AgainstNull(entityType, "entityType");

            return entityType
                .GetHierarchy()
                .SelectMany(GetFieldsDefinedIn);
        }

        private static IEnumerable<CopyableFieldDescription> GetFieldsDefinedIn(Type type)
        {
            MemberInfo[] typeMembers = type
                .GetMembers(BindingFlags.Public | BindingFlags.NonPublic
                            | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            IEnumerable<CopyableFieldDescription> fieldsInfo = typeMembers
                .OfType<FieldInfo>()
                .Where(field => !field.IsBackingField())
                .Select(field => new CopyableFieldDescription(field));

            IEnumerable<CopyableFieldDescription> propertiesInfo = typeMembers
                .OfType<PropertyInfo>()
                .Where(prop => prop.IsAutoProperty())
                .Select(prop => new CopyableFieldDescription(prop, prop.GetBackingField()));

            return propertiesInfo.Concat(fieldsInfo);
        }

        /// <summary>
        /// Description of one field in replicated type.
        /// </summary>
        /// <remarks>
        /// The main need of these class is to bind compiler-generated fields to 
        /// auto-implemented properties that causes their generation.
        /// </remarks>
        // marked internal for testing purposes
        protected internal class CopyableFieldDescription
        {
            public CopyableFieldDescription(FieldInfo field)
            {
                BackingField = field;
                DeclaringMember = field;
            }

            public CopyableFieldDescription(PropertyInfo autoProperty, FieldInfo backingField)
            {
                BackingField = backingField;
                DeclaringMember = autoProperty;
            }

            public FieldInfo BackingField { get; private set; }
            public MemberInfo DeclaringMember { get; private set; }
        }
    }
}