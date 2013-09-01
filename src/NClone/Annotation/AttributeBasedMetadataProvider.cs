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
    public class AttributeBasedMetadataProvider: IMetadataProvider
    {
        public ReplicationBehavior GetBehavior(Type entityType)
        {
            Guard.AgainstNull(entityType, "entityType");

            CustomReplicationBehaviorAttribute customReplicationBehavior;
            if (entityType
                .GetCustomAttributes<CustomReplicationBehaviorAttribute>()
                .TrySingle(out customReplicationBehavior))
                return customReplicationBehavior.ReplicationBehavior;

            if (typeof (Delegate).IsAssignableFrom(entityType))
                return ReplicationBehavior.Ignore;
            if (entityType.IsValueType)
                return ReplicationBehavior.Copy;
            return ReplicationBehavior.DeepCopy;
        }

        public IEnumerable<Tuple<FieldInfo, ReplicationBehavior>> GetMembers(Type entityType)
        {
            return entityType
                .GetHierarchy()
                .SelectMany(t => t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                .DistinctBy(f => f.MetadataToken)
                .Select(f => Tuple.Create(f, GetBehavior(f)));
        }

        private static ReplicationBehavior GetBehavior(FieldInfo fieldInfo)
        {
            CustomReplicationBehaviorAttribute customReplicationBehavior;
            return fieldInfo.GetCustomAttributes<CustomReplicationBehaviorAttribute>()
                            .TrySingle(out customReplicationBehavior)
                ? customReplicationBehavior.ReplicationBehavior
                : ReplicationBehavior.DeepCopy;
        }
    }
}