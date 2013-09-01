﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using NClone.MemberAccess;
using NClone.Shared;

namespace NClone.SecondVersion
{
    /// <summary>
    /// Implementation of <see cref="IEntityReplicator"/> for general reference types.
    /// </summary>
    internal class ReferenceTypeReplicator: IEntityReplicator
    {
        private readonly Type type;
        private readonly IEnumerable<IMemberAccessor> memberAccessors;

        public ReferenceTypeReplicator(Type type)
        {
            Guard.AgainstNull(type, "type");
            Guard.AgainstViolation(!type.IsValueType, "Type should be reference type");

            this.type = type;
            memberAccessors = type
                .GetHierarchy()
                .SelectMany(t => t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                .DistinctBy(x => x.MetadataToken)
                .Select(field => FieldAccessorBuilder.BuildFor(type, field, true))
                .Materialize();
        }

        public object Replicate(object source)
        {
            var result = FormatterServices.GetUninitializedObject(type);
            foreach (var memberAccessor in memberAccessors) {
                var memberValue = memberAccessor.GetMember(source);
                var replicatedValue = ObjectReplicator.Replicate(memberValue);
                memberAccessor.SetMember(result, replicatedValue);
            }
            return result;
        }
    }
}