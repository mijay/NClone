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
    /// <para><see cref="DefaultMetadataProvider"/> defines that:</para>
    /// <para>1) Primitive types, enums and <c>string</c>s should be just <see cref="ReplicationBehavior.Copy"/>.</para>
    /// <para>2) <see cref="Nullable{T}"/> types inherit <see cref="ReplicationBehavior"/> from underlying type.</para>
    /// <para>3) When type is replicated, all its fields should be replicated.</para>
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
            return GetAllFields(type).Select(x => new FieldReplicationInfo(x, ReplicationBehavior.Replicate));
        }

        protected bool TryGetDefaultBehavior(Type entityType, out ReplicationBehavior behavior)
        {
            Guard.AgainstNull(entityType, "entityType");

            if (entityType.IsPrimitive || entityType.IsEnum
                || entityType == typeof (string) || entityType == typeof (object)) {
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

        protected static IEnumerable<FieldInfo> GetAllFields(Type entityType)
        {
            Guard.AgainstNull(entityType, "entityType");

            return entityType
                .GetHierarchy()
                .SelectMany(t => t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                .DistinctBy(f => f.MetadataToken);
        }
    }
}