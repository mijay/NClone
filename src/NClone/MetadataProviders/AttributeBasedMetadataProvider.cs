﻿using System;
using System.Reflection;
using NClone.Utils;

namespace NClone.MetadataProviders
{
    /// <summary>
    /// Implementation of <see cref="IMetadataProvider"/> that uses information from <see cref="CustomReplicationBehaviorAttribute"/>s.
    /// </summary>
    /// <remarks>
    /// <para>Method <see cref="IMetadataProvider.GetPerTypeBehavior"/> consider type-level
    /// <see cref="CustomReplicationBehaviorAttribute"/>s, while method
    /// <see cref="IMetadataProvider.GetFieldsReplicationInfo"/> consider only member-level attributes.</para>
    /// 
    /// <para>Note that <see cref="CustomReplicationBehaviorAttribute"/> has no effect on common properties,
    /// it only affects auto-properties.</para>
    /// </remarks>
    /// <seealso cref="CustomReplicationBehaviorAttribute"/>
    public class AttributeBasedMetadataProvider: DefaultMetadataProvider
    {
        protected override ReplicationBehavior? TryGetPerTypeBehavior(Type type)
        {
            return base.TryGetPerTypeBehavior(type)
                   ?? TryGetBehaviorFromAttribute(type);
        }

        protected override ReplicationBehavior? TryGetPerMemberReplicationBehavior(CopyableFieldDescription fieldDescription)
        {
            return base.TryGetPerMemberReplicationBehavior(fieldDescription)
                   ?? TryGetBehaviorFromAttribute(fieldDescription.DeclaringMember);
        }

        /// <summary>
        /// Returns <see cref="ReplicationBehavior"/> defined via <see cref="CustomReplicationBehaviorAttribute"/>
        /// on specific <paramref name="memberOrType"/> or <c>null</c> if no attribute is specified.
        /// </summary>
        protected static ReplicationBehavior? TryGetBehaviorFromAttribute(MemberInfo memberOrType)
        {
            CustomReplicationBehaviorAttribute customReplicationBehavior;
            if (memberOrType.GetCustomAttributes<CustomReplicationBehaviorAttribute>()
                .TrySingle(out customReplicationBehavior))
                return customReplicationBehavior.GetReplicationBehavior();
            return null;
        }
    }
}