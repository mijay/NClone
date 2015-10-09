using System;
using System.Linq;
using System.Runtime.Serialization;
using mijay.Utils;
using mijay.Utils.Reflection;
using NClone.MemberAccess;
using NClone.MetadataProviders;
using NClone.ObjectReplication;

namespace NClone.ReplicationStrategies
{
    /// <summary>
    /// Implementation of <see cref="IReplicationStrategy"/> for value types.
    /// </summary>
    internal class ValueTypeReplicationStrategy: IReplicationStrategy
    {
        private readonly Type entityType;
        private readonly MemberReplicationInfo[] memberDescriptions;

        public ValueTypeReplicationStrategy(IMetadataProvider metadataProvider, Type entityType)
        {
            Guard.AgainstNull(metadataProvider, "metadataProvider");
            Guard.AgainstNull(entityType, "entityType");
            Guard.AgainstViolation(entityType.IsValueType && !entityType.IsNullable(),
                "ValueTypeReplicationStrategy is not applicable to nullable or by-ref types");

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
            foreach (var memberReplicationInfo in memberDescriptions) {
                object memberValue = memberReplicationInfo.GetMember(source);
                object replicatedValue = memberReplicationInfo.Behavior == ReplicationBehavior.DeepCopy
                    ? context.ReplicateAsync(memberValue)
                    : memberValue;
                result = memberReplicationInfo.SetMember(result, replicatedValue);
            }
            return result;
        }

        private struct MemberReplicationInfo
        {
            public readonly ReplicationBehavior Behavior;
            public readonly Func<object, object> GetMember;
            public readonly Func<object, object, object> SetMember;

            public MemberReplicationInfo(Type containerType, FieldReplicationInfo fieldReplicationInfo)
            {
                Behavior = fieldReplicationInfo.Behavior;
                IMemberAccessor memberAccessor = FieldAccessorBuilder.BuildFor(containerType, fieldReplicationInfo.Field, true);
                GetMember = memberAccessor.GetMember;
                SetMember = memberAccessor.SetMember;
            }
        }
    }
}