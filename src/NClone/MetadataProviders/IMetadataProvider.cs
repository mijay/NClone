using System;
using System.Collections.Generic;
using System.Reflection;

namespace NClone.MetadataProviders
{
    /// <summary>
    /// Provides information about <see cref="ReplicationBehavior"/> for types and type members.
    /// </summary>
    /// <remarks>
    /// <para><see cref="IMetadataProvider"/> provides two levels of <see cref="ReplicationBehavior"/>:
    /// defined per-type (can be obtained via <see cref="GetPerTypeBehavior"/>), and defined per-member
    /// (can be obtained via <see cref="GetFieldsReplicationInfo"/>). Actual <see cref="ReplicationBehavior"/> for
    /// specific replicating object is computed as a minimum of its per-type <see cref="ReplicationBehavior"/>
    /// and <see cref="ReplicationBehavior"/> for member, in which it is stored.</para>
    /// 
    /// <para>Such approach gives additional flexibility for declaring type to adjust how its members are replicated.</para>
    /// </remarks>
    public interface IMetadataProvider
    {
        /// <summary>
        /// Provides per-type <see cref="ReplicationBehavior"/> for given <paramref name="type"/>.
        /// </summary>
        ReplicationBehavior GetPerTypeBehavior(Type type);

        /// <summary>
        /// Get the list of <see cref="FieldInfo"/>s in concrete type and per-member <see cref="ReplicationBehavior"/>
        /// defined for them.
        /// </summary>
        IEnumerable<FieldReplicationInfo> GetFieldsReplicationInfo(Type type);
    }
}