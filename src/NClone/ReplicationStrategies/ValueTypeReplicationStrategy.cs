using System;
using mijay.Utils;
using mijay.Utils.Reflection;
using NClone.MetadataProviders;
using NClone.ObjectReplication;

namespace NClone.ReplicationStrategies
{
    /// <summary>
    /// Implementation of <see cref="IReplicationStrategy"/> for value types.
    /// </summary>
    internal class ValueTypeReplicationStrategy: TypeReplicationStrategyBase
    {
        public ValueTypeReplicationStrategy(IMetadataProvider metadataProvider, Type entityType): base(metadataProvider, entityType)
        {
            Guard.AgainstViolation(entityType.IsValueType && !entityType.IsNullable(),
                "ValueTypeReplicationStrategy is not applicable to nullable or by-ref types");
        }

        protected override object PopulateObject(object target, object source, IReplicationContext context)
        {
            foreach (var memberReplicationInfo in memberDescriptions)
            {
                object memberValue = memberReplicationInfo.getMember(source);
                switch (memberReplicationInfo.behavior)
                {
                case ReplicationBehavior.Copy:
                    target = memberReplicationInfo.setMember(target, memberValue);
                    break;
                case ReplicationBehavior.DeepCopy:
                    var replicatedValueAsync = context.ReplicateAsync(memberValue);
                    if (!replicatedValueAsync.IsCompleted)
                        throw ReplicationException.CycleThroughValueTypeFound();
                    target = memberReplicationInfo.setMember(target, replicatedValueAsync.Result);
                    break;
                }
            }
            return target;
        }
    }
}
