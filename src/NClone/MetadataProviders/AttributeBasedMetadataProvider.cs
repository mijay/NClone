using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NClone.Shared;

namespace NClone.MetadataProviders
{
    /// <summary>
    /// Implementation of <see cref="IMetadataProvider"/> that uses information from <see cref="CustomReplicationBehaviorAttribute"/>s.
    /// </summary>
    /// <remarks>
    /// <para>Method <see cref="GetPerTypeBehavior"/> consider type-level <see cref="CustomReplicationBehaviorAttribute"/>s,
    /// while method <see cref="GetFieldsReplicationInfo"/> consider member-level <see cref="CustomReplicationBehaviorAttribute"/>s.</para>
    /// 
    /// <para>Note that <see cref="CustomReplicationBehaviorAttribute"/> has no effect on common properties,
    /// it only affects auto-properties.</para>
    /// </remarks>
    /// <seealso cref="CustomReplicationBehaviorAttribute"/>
    public class AttributeBasedMetadataProvider: DefaultMetadataProvider
    {
        public override ReplicationBehavior GetPerTypeBehavior(Type type)
        {
            ReplicationBehavior behavior;
            if (TryGetDefaultBehavior(type, out behavior))
                return behavior;
            if (TryGetBehaviorFromTypeAttribute(type, out behavior))
                return behavior;
            return ReplicationBehavior.Replicate;
        }

        protected static bool TryGetBehaviorFromTypeAttribute(Type entityType, out ReplicationBehavior behavior)
        {
            CustomReplicationBehaviorAttribute customReplicationBehavior;
            if (entityType.GetCustomAttributes<CustomReplicationBehaviorAttribute>()
                .TrySingle(out customReplicationBehavior)) {
                behavior = customReplicationBehavior.GetReplicationBehavior();
                return true;
            }
            behavior = ReplicationBehavior.Replicate;
            return false;
        }

        public override IEnumerable<FieldReplicationInfo> GetFieldsReplicationInfo(Type type)
        {
            return type
                .GetHierarchy()
                .SelectMany(GetMembersDefinedIn);
        }

        private static IEnumerable<FieldReplicationInfo> GetMembersDefinedIn(Type type)
        {
            IEnumerable<FieldReplicationInfo> fieldsInfo = type
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(field => field.DeclaringType == type)
                .Where(field => !field.IsBackingField())
                .Select(field => new FieldReplicationInfo(field, GetBehaviorOrDefault(field)));

            IEnumerable<FieldReplicationInfo> propertiesInfo = type
                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(prop => prop.DeclaringType == type)
                .Where(prop => prop.IsAutoProperty())
                .Select(prop => new FieldReplicationInfo(prop.GetBackingField(), GetBehaviorOrDefault(prop)));

            return propertiesInfo.Concat(fieldsInfo);
        }

        private static ReplicationBehavior GetBehaviorOrDefault(MemberInfo memberInfo)
        {
            CustomReplicationBehaviorAttribute customReplicationBehavior;
            if (memberInfo.GetCustomAttributes<CustomReplicationBehaviorAttribute>()
                .TrySingle(out customReplicationBehavior))
                return customReplicationBehavior.GetReplicationBehavior();
            return ReplicationBehavior.Replicate;
        }
    }
}