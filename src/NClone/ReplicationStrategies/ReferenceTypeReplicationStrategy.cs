using System;
using System.Linq;
using System.Runtime.Serialization;
using mijay.Utils;
using mijay.Utils.Tasks;
using NClone.MemberAccess;
using NClone.MetadataProviders;
using NClone.ObjectReplication;

namespace NClone.ReplicationStrategies
{
    /// <summary>
    /// Implementation of <see cref="IReplicationStrategy"/> for general reference types.
    /// </summary>
    internal class ReferenceTypeReplicationStrategy: IReplicationStrategy
    {
        private readonly Type entityType;
        private readonly MemberReplicationInfo[] memberDescriptions;

        public ReferenceTypeReplicationStrategy(IMetadataProvider metadataProvider, Type entityType)
        {
            Guard.AgainstNull(metadataProvider, "metadataProvider");
            Guard.AgainstNull(entityType, "entityType");
            Guard.AgainstViolation(!entityType.IsValueType && !entityType.IsArray,
                "ReferenceTypeReplicationStrategy is applicable only to by-ref types except arrays");

            this.entityType = entityType;

            memberDescriptions = metadataProvider.GetFieldsReplicationInfo(entityType)
                .Where(t => t.Behavior != ReplicationBehavior.Ignore)
                .Select(t => new MemberReplicationInfo(entityType, t))
                .ToArray();
        }

        public object Replicate(object source, IReplicationContext context)
        {
            Guard.AgainstViolation(source.GetType() == entityType,
                "This replicator can copy only entities of type {0}, but {1} received",
                entityType, source.GetType());

            object result = FormatterServices.GetUninitializedObject(entityType);
            foreach (var memberReplicationInfo in memberDescriptions)
            {
                object memberValue = memberReplicationInfo.GetMember(source);
                if (memberReplicationInfo.Behavior == ReplicationBehavior.DeepCopy)
                {
                    context.ReplicateAsync(memberValue)
                        .Then(replicatedValue => memberReplicationInfo.SetMember(result, replicatedValue));
                }
                else
                    memberReplicationInfo.SetMember(result, memberValue);
            }
            return result;
        }

        private struct MemberReplicationInfo
        {
            public readonly ReplicationBehavior Behavior;
            public readonly Func<object, object> GetMember;
            public readonly Action<object, object> SetMember;

            public MemberReplicationInfo(Type containerType, FieldReplicationInfo fieldReplicationInfo)
            {
                Behavior = fieldReplicationInfo.Behavior;
                IMemberAccessor memberAccessor = FieldAccessorBuilder.BuildFor(containerType, fieldReplicationInfo.Field, true);
                GetMember = memberAccessor.GetMember;
                SetMember = (obj, val) => memberAccessor.SetMember(obj, val);
            }
        }
    }
}
