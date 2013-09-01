using System;
using System.Collections.Generic;
using System.Reflection;

namespace NClone.Annotation
{
    /// <summary>
    /// Provides information about <see cref="ReplicationBehavior"/> for fields and types.
    /// </summary>
    public interface IMetadataProvider
    {
        ReplicationBehavior GetBehavior(Type entityType);
        IEnumerable<Tuple<FieldInfo, ReplicationBehavior>> GetMembers(Type entityType);
    }
}