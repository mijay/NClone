using System;
using System.Collections.Generic;
using System.Reflection;

namespace NClone.Annotation
{
    public interface IMetadataProvider
    {
        IEnumerable<FieldInfo> GetReplicatingMembers(Type entityType);
    }
}