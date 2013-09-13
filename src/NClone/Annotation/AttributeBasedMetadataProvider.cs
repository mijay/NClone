using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NClone.Shared;

namespace NClone.Annotation
{
    /// <summary>
    /// Implementation of <see cref="IMetadataProvider"/> that uses information from <see cref="CustomReplicationBehaviorAttribute"/>s.
    /// </summary>
    public class AttributeBasedMetadataProvider: BasicMetadataProvider
    {
        public override ReplicationBehavior GetBehavior(Type entityType)
        {
            ReplicationBehavior behavior;
            if (TryGetDefaultBehavior(entityType, out behavior))
                return behavior;
            if (TryGetBehaviorFromTypeAttribute(entityType, out behavior))
                return behavior;
            return ReplicationBehavior.DeepCopy;
        }

        protected static bool TryGetBehaviorFromTypeAttribute(Type entityType, out ReplicationBehavior behavior)
        {
            CustomReplicationBehaviorAttribute customReplicationBehavior;
            if (entityType.GetCustomAttributes<CustomReplicationBehaviorAttribute>()
                          .TrySingle(out customReplicationBehavior)) {
                behavior = customReplicationBehavior.ReplicationBehavior;
                return true;
            }
            behavior = ReplicationBehavior.DeepCopy;
            return false;
        }

        public override IEnumerable<Tuple<FieldInfo, ReplicationBehavior>> GetMembers(Type entityType)
        {
            return GetAllFields(entityType)
                .Select(field => {
                    ReplicationBehavior behavior;
                    if (!TryGetBehaviorFromAttribute(field, out behavior))
                        behavior = ReplicationBehavior.DeepCopy;
                    return Tuple.Create(field, behavior);
                });
        }

        protected static bool TryGetBehaviorFromAttribute(FieldInfo fieldInfo, out ReplicationBehavior behavior)
        {
            CustomReplicationBehaviorAttribute customReplicationBehavior;
            if (fieldInfo.GetCustomAttributes<CustomReplicationBehaviorAttribute>()
                         .TrySingle(out customReplicationBehavior)) {
                behavior = customReplicationBehavior.ReplicationBehavior;
                return true;
            }
            behavior = ReplicationBehavior.DeepCopy;
            return false;
        }
    }
}