using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NClone.Shared;

namespace NClone.Annotation
{
    /// <summary>
    /// Abstract implementation of <see cref="IMetadataProvider"/> that provides basic helper methods.
    /// </summary>
    public abstract class MetadataProviderBase: IMetadataProvider
    {
        public abstract ReplicationBehavior GetBehavior(Type entityType);
        public abstract IEnumerable<Tuple<FieldInfo, ReplicationBehavior>> GetMembers(Type entityType);

        protected static IEnumerable<FieldInfo> GetAllFields(Type entityType)
        {
            Guard.AgainstNull(entityType, "entityType");

            return entityType
                .GetHierarchy()
                .SelectMany(t => t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                .DistinctBy(f => f.MetadataToken);
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
    }
}