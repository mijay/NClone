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
        public ReplicationBehavior GetPerTypeBehavior(Type type)
        {
            Guard.AgainstNull(type, "type");
            return TryGetPerTypeBehavior(type) ?? ReplicationBehavior.Replicate;
        }

        public IEnumerable<FieldReplicationInfo> GetFieldsReplicationInfo(Type type)
        {
            Guard.AgainstNull(type, "type");

            return type.GetHierarchy()
                .SelectMany(GetFieldsDefinedIn)
                .Select(x => new FieldReplicationInfo(x.BackingField,
                    TryGetPerMemberReplicationBehavior(x) ?? ReplicationBehavior.Replicate));
        }

        /// <summary>
        /// Extension point. Try to find per-type <see cref="ReplicationBehavior"/> or
        /// return <c>null</c>, if current <see cref="IMetadataProvider"/> does not specify it.
        /// </summary>
        protected virtual ReplicationBehavior? TryGetPerTypeBehavior(Type entityType)
        {
            if (entityType.IsPrimitive || entityType.IsEnum || entityType == typeof (string))
                return ReplicationBehavior.Copy;
            if (entityType.IsNullable()) {
                Type underlyingType = entityType.GetNullableUnderlyingType();
                return GetPerTypeBehavior(underlyingType);
            }
            return null;
        }

        /// <summary>
        /// Extension point. Try to find per-member <see cref="ReplicationBehavior"/> or
        /// return <c>null</c>, if current <see cref="IMetadataProvider"/> does not specify it.
        /// </summary>
        protected virtual ReplicationBehavior? TryGetPerMemberReplicationBehavior(CopyableFieldDescription fieldDescription)
        {
            return null;
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