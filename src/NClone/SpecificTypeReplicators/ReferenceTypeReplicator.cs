using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using NClone.Annotation;
using NClone.MemberAccess;
using NClone.ObjectReplicators;
using NClone.Shared;

namespace NClone.SpecificTypeReplicators
{
    /// <summary>
    /// Implementation of <see cref="ISpecificTypeReplicator"/> for general reference types.
    /// </summary>
    internal class ReferenceTypeReplicator: ISpecificTypeReplicator
    {
        private readonly IMetadataProvider metadataProvider;
        private readonly IObjectReplicator objectReplicator;
        private readonly Type entityType;
        private readonly IEnumerable<IMemberAccessor> memberAccessors;

        public ReferenceTypeReplicator(IMetadataProvider metadataProvider, IObjectReplicator objectReplicator, Type entityType)
        {
            Guard.AgainstNull(metadataProvider, "metadataProvider");
            Guard.AgainstNull(objectReplicator, "objectReplicator");
            Guard.AgainstNull(entityType, "entityType");
            Guard.AgainstViolation(!entityType.IsValueType, "Type should be reference type");

            this.metadataProvider = metadataProvider;
            this.objectReplicator = objectReplicator;
            this.entityType = entityType;
            memberAccessors = entityType
                .GetHierarchy()
                .SelectMany(t => t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                .DistinctBy(x => x.MetadataToken)
                .Select(field => FieldAccessorBuilder.BuildFor(entityType, field, true))
                .Materialize();
        }

        public object Replicate(object source)
        {
            Guard.AgainstNull(source, "source");
            Guard.AgainstViolation(source.GetType() == entityType,
                "This replicator can copy only entities of type {0}, but {1} received",
                entityType, source.GetType());

            var result = FormatterServices.GetUninitializedObject(entityType);
            foreach (var memberAccessor in memberAccessors) {
                var memberValue = memberAccessor.GetMember(source);
                var replicatedValue = objectReplicator.Replicate(memberValue);
                memberAccessor.SetMember(result, replicatedValue);
            }
            return result;
        }
    }
}