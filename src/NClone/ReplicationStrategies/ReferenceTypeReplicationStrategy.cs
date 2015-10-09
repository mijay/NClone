using System;
using mijay.Utils;
using mijay.Utils.Tasks;
using NClone.MetadataProviders;
using NClone.ObjectReplication;

namespace NClone.ReplicationStrategies
{
    /// <summary>
    /// Implementation of <see cref="IReplicationStrategy"/> for general reference types.
    /// </summary>
    internal class ReferenceTypeReplicationStrategy: TypeReplicationStrategyBase
    {
        public ReferenceTypeReplicationStrategy(IMetadataProvider metadataProvider, Type entityType): base(metadataProvider, entityType)
        {
            Guard.AgainstViolation(!entityType.IsValueType && !entityType.IsArray,
                "ReferenceTypeReplicationStrategy is applicable only to by-ref types except arrays");
        }

        protected override object PopulateObject(object target, object source, IReplicationContext context)
        {
            foreach (var memberReplicationInfo in memberDescriptions)
            {
                object memberValue = memberReplicationInfo.getMember(source);
                switch (memberReplicationInfo.behavior)
                {
                case ReplicationBehavior.Copy:
                    memberReplicationInfo.setMember(target, memberValue);
                    break;
                case ReplicationBehavior.DeepCopy:
                    context.ReplicateAsync(memberValue).Then(replicatedValue => memberReplicationInfo.setMember(target, replicatedValue));
                    break;
                }
            }
            return target;
        }
    }
}
