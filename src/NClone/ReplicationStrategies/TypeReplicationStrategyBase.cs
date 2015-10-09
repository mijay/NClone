using System;
using System.Linq;
using System.Runtime.Serialization;
using mijay.Utils;
using NClone.MemberAccess;
using NClone.MetadataProviders;
using NClone.ObjectReplication;

namespace NClone.ReplicationStrategies
{
    /// <summary>
    /// Base class for implementing <see cref="IReplicationStrategy"/> for concrete types containing members.
    /// </summary>
    internal abstract class TypeReplicationStrategyBase: IReplicationStrategy
    {
        private readonly Type entityType;
        protected readonly MemberReplicationInfo[] memberDescriptions;

        protected TypeReplicationStrategyBase(IMetadataProvider metadataProvider, Type entityType)
        {
            Guard.AgainstNull(metadataProvider, "metadataProvider");
            Guard.AgainstNull(entityType, "entityType");

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
            return PopulateObject(result, source, context);
        }

        protected abstract object PopulateObject(object target, object source, IReplicationContext context);

        protected struct MemberReplicationInfo
        {
            public readonly ReplicationBehavior behavior;
            public readonly Func<object, object> getMember;
            public readonly Func<object, object, object> setMember;

            public MemberReplicationInfo(Type containerType, FieldReplicationInfo fieldReplicationInfo)
            {
                behavior = fieldReplicationInfo.Behavior;
                IMemberAccessor memberAccessor = FieldAccessorBuilder.BuildFor(containerType, fieldReplicationInfo.Field, true);
                getMember = memberAccessor.GetMember;
                setMember = memberAccessor.SetMember;
            }
        }
    }
}
