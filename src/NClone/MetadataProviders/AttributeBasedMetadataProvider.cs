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
    /// <para>Method <see cref="GetBehavior"/> consider type-level <see cref="CustomReplicationBehaviorAttribute"/>s,
    /// while method <see cref="GetMembers"/> consider member-level <see cref="CustomReplicationBehaviorAttribute"/>s.</para>
    /// 
    /// <para>Note that <see cref="CustomReplicationBehaviorAttribute"/> has no effect on common properties,
    /// it only affects auto-properties.</para>
    /// </remarks>
    /// <seealso cref="CustomReplicationBehaviorAttribute"/>
    public class AttributeBasedMetadataProvider: DefaultMetadataProvider
    {
        public override ReplicationBehavior GetBehavior(Type entityType)
        {
            ReplicationBehavior behavior;
            if (TryGetDefaultBehavior(entityType, out behavior))
                return behavior;
            if (TryGetBehaviorFromTypeAttribute(entityType, out behavior))
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

        public override IEnumerable<MemberInformation> GetMembers(Type entityType)
        {
            return GetAllFields(entityType)
                .Select(field => {
                            ReplicationBehavior behavior;
                            if (!TryGetBehaviorFromAttribute(field, out behavior))
                                behavior = ReplicationBehavior.Replicate;
                            return new MemberInformation(field, behavior);
                        });
        }

        protected static bool TryGetBehaviorFromAttribute(FieldInfo fieldInfo, out ReplicationBehavior behavior)
        {
            CustomReplicationBehaviorAttribute customReplicationBehavior;
            if (fieldInfo.GetCustomAttributes<CustomReplicationBehaviorAttribute>()
                .TrySingle(out customReplicationBehavior)) {
                behavior = customReplicationBehavior.GetReplicationBehavior();
                return true;
            }
            behavior = ReplicationBehavior.Replicate;
            return false;
        }
    }
}