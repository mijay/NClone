using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using NClone.MemberAccess;
using NClone.MetadataProviders;
using NClone.ObjectReplication;
using NClone.Utils;

namespace NClone.ReplicationStrategies
{
    /// <summary>
    /// Implementation of <see cref="IReplicationStrategy"/> for general reference or value types.
    /// </summary>
    internal class CommonReplicationStrategy: IReplicationStrategy
    {
        private readonly Type entityType;
        private readonly IEnumerable<Tuple<ReplicationBehavior, IMemberAccessor>> memberDescriptions;

        public CommonReplicationStrategy(IMetadataProvider metadataProvider, Type entityType)
        {
            Guard.AgainstNull(metadataProvider, "metadataProvider");
            Guard.AgainstNull(entityType, "entityType");
            Guard.AgainstViolation(!entityType.IsNullable(), "CommonReplicationStrategy is not applicable to nullable types");

            this.entityType = entityType;

            memberDescriptions = metadataProvider.GetFieldsReplicationInfo(entityType)
                .Where(t => t.Behavior != ReplicationBehavior.Ignore)
                .Select(t => Tuple.Create(t.Behavior, FieldAccessorBuilder.BuildFor(entityType, t.Member, true)))
                .Materialize();
        }

        public object Replicate(object source, IReplicationContext context)
        {
            Guard.AgainstNull(source, "source");
            Guard.AgainstViolation(source.GetType() == entityType,
                "This replicator can copy only entities of type {0}, but {1} received",
                entityType, source.GetType());

            object result = FormatterServices.GetUninitializedObject(entityType);
            foreach (var memberDescription in memberDescriptions) {
                object memberValue = memberDescription.Item2.GetMember(source);
                object replicatedValue = memberDescription.Item1 == ReplicationBehavior.Replicate
                    ? context.Replicate(memberValue)
                    : memberValue;
                result = memberDescription.Item2.SetMember(result, replicatedValue);
            }
            return result;
        }
    }
}