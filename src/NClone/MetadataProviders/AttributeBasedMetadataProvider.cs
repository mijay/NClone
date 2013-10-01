using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NClone.Shared;

namespace NClone.MetadataProviders
{
    /// <summary>
    /// Implementation of <see cref="IMetadataProvider"/> that uses information from <see cref="CustomReplicationBehaviorAttribute"/>s.
    /// </summary>
    /// <remarks>
    /// <para>Method <see cref="GetPerTypeBehavior"/> consider type-level <see cref="CustomReplicationBehaviorAttribute"/>s,
    /// while method <see cref="GetFieldsReplicationInfo"/> consider member-level <see cref="CustomReplicationBehaviorAttribute"/>s.</para>
    /// 
    /// <para>Note that <see cref="CustomReplicationBehaviorAttribute"/> has no effect on common properties,
    /// it only affects auto-properties.</para>
    /// </remarks>
    /// <seealso cref="CustomReplicationBehaviorAttribute"/>
    public class AttributeBasedMetadataProvider: DefaultMetadataProvider
    {
        public override ReplicationBehavior GetPerTypeBehavior(Type type)
        {
            ReplicationBehavior behavior;
            if (TryGetDefaultBehavior(type, out behavior))
                return behavior;
            if (TryGetBehaviorFromAttribute(type, out behavior))
                return behavior;
            return ReplicationBehavior.Replicate;
        }

        public override IEnumerable<FieldReplicationInfo> GetFieldsReplicationInfo(Type type)
        {
            return GetAllFields(type)
                .Select(x => {
                            ReplicationBehavior behavior;
                            if (!TryGetBehaviorFromAttribute(x.DeclaringMember, out behavior))
                                behavior = ReplicationBehavior.Replicate;
                            return new FieldReplicationInfo(x.BackingField, behavior);
                        });
        }

        protected static bool TryGetBehaviorFromAttribute(MemberInfo memberOrType, out ReplicationBehavior behavior)
        {
            CustomReplicationBehaviorAttribute customReplicationBehavior;
            if (memberOrType.GetCustomAttributes<CustomReplicationBehaviorAttribute>()
                .TrySingle(out customReplicationBehavior)) {
                behavior = customReplicationBehavior.GetReplicationBehavior();
                return true;
            }
            behavior = ReplicationBehavior.Replicate;
            return false;
        }
    }
}