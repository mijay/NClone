using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NClone.Shared;

namespace NClone.Annotation
{
    /// <summary>
    /// Implementation of <see cref="IMetadataProvider"/> that provides only basic functionality.
    /// </summary>
    public class BasicMetadataProvider: IMetadataProvider
    {
        public virtual ReplicationBehavior GetBehavior(Type entityType)
        {
            ReplicationBehavior behavior;
            if (TryGetDefaultBehavior(entityType, out behavior))
                return behavior;
            return ReplicationBehavior.DeepCopy;
        }

        protected static bool TryGetDefaultBehavior(Type entityType, out ReplicationBehavior behavior)
        {
            Guard.AgainstNull(entityType, "entityType");

            if (entityType.IsPrimitive || entityType.IsEnum || entityType == typeof (string)) {
                behavior = ReplicationBehavior.Ignore;
                return true;
            }
            if (entityType == typeof (object)) {
                behavior = ReplicationBehavior.Copy;
                return true;
            }
            behavior = ReplicationBehavior.DeepCopy;
            return false;
        }

        public virtual IEnumerable<Tuple<FieldInfo, ReplicationBehavior>> GetMembers(Type entityType)
        {
            return GetAllFields(entityType).Select(x => Tuple.Create(x, ReplicationBehavior.DeepCopy));
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