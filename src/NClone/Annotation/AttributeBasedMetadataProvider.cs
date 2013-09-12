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
    public class AttributeBasedMetadataProvider: MetadataProviderBase
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
            return GetAllFields(entityType).Select(f => Tuple.Create(f, GetFieldBehavior(f)));
        }

        private static ReplicationBehavior GetFieldBehavior(FieldInfo fieldInfo)
        {
            CustomReplicationBehaviorAttribute customReplicationBehavior;
            return fieldInfo.GetCustomAttributes<CustomReplicationBehaviorAttribute>()
                            .TrySingle(out customReplicationBehavior)
                ? customReplicationBehavior.ReplicationBehavior
                : ReplicationBehavior.DeepCopy;
        }
    }
}