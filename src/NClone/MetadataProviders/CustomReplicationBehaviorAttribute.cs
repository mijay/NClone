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
    /// <para>When applied to auto-implemented properties actually affects their backing fields. When applied to normal properties
    /// does not have any affect.</para>
    /// 
    /// <para>To apply to backing properties of auto-implemented events use the following syntax:
    /// <code>
    /// [field: CustomReplicationBehavior(ReplicationBehavior.Ignore)]
    /// public event Action SomeEvent;
    /// </code></para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property,
        AllowMultiple = false, Inherited = true)]
    public sealed class CustomReplicationBehaviorAttribute: Attribute
    {
        private readonly ReplicationBehavior replicationBehavior;

        public CustomReplicationBehaviorAttribute(ReplicationBehavior replicationBehavior)
        {
            this.replicationBehavior = replicationBehavior;
        }

        public ReplicationBehavior GetReplicationBehavior()
        {
            return replicationBehavior;
        }
    }
}