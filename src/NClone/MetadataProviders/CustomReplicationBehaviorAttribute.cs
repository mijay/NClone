using System;

namespace NClone.MetadataProviders
{
    /// <summary>
    /// Attribute that can be used to specify concrete <see cref="ReplicationBehavior"/> for types and type members.
    /// </summary>
    /// <remarks>
    /// <para>When applied to types, it will affect the per-type behavior (provided via <see cref="IMetadataProvider.GetPerTypeBehavior"/>.
    /// When applied to type members, it will affect per-member behavior (provided via <see cref="IMetadataProvider.GetFieldsReplicationInfo"/>.</para>
    /// 
    /// <para>When applied to auto-implemented properties or events actually affects their backing fields. When applied to normal properties
    /// or events does not have any affect.</para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct |
                    AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Event,
        AllowMultiple = false, Inherited = true)]
    public sealed class CustomReplicationBehaviorAttribute: Attribute
    {
        private readonly ReplicationBehavior replicationBehavior;

        /// <summary>
        /// Constructor for <see cref="CustomReplicationBehaviorAttribute"/>.
        /// </summary>
        public CustomReplicationBehaviorAttribute(ReplicationBehavior replicationBehavior)
        {
            this.replicationBehavior = replicationBehavior;
        }

        /// <summary>
        /// Returns <see cref="ReplicationBehavior"/> assigned to member via
        /// current <see cref="CustomReplicationBehaviorAttribute"/>.
        /// </summary>
        public ReplicationBehavior GetReplicationBehavior()
        {
            return replicationBehavior;
        }
    }
}