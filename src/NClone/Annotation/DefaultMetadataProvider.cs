using System;
using System.Collections.Generic;
using System.Reflection;

namespace NClone.Annotation
{
    /// <summary>
    /// Default implementation of <see cref="IMetadataProvider"/>.
    /// </summary>
    public class DefaultMetadataProvider: IMetadataProvider
    {
        public IEnumerable<FieldInfo> GetReplicatingMembers(Type entityType)
        {
            return entityType
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }
    }
}