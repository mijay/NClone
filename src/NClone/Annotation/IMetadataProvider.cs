using System;
using System.Collections.Generic;

namespace NClone.Annotation
{
    /// <summary>
    /// Provides information about <see cref="ReplicationBehavior"/> for fields and types.
    /// </summary>
    public interface IMetadataProvider
    {
        /// <summary>
        /// Provides class-level <see cref="ReplicationBehavior"/> for given <paramref name="entityType"/>.
        /// </summary>
        ReplicationBehavior GetBehavior(Type entityType);

        /// <summary>
        /// Provides list of type members and declaration-side behavior for them.
        /// </summary>
        IEnumerable<MemberInformation> GetMembers(Type entityType);
    }
}