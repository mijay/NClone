using System;
using System.Collections;
using System.Collections.Generic;
using NClone.Shared;

namespace NClone.MetadataProviders
{
    /// <summary>
    /// Implementation of <see cref="IMetadataProvider"/> that is based on conventions
    /// and information from <see cref="CustomReplicationBehaviorAttribute"/>s.
    /// </summary>
    /// <remarks>
    /// <para>Used conventions:</para>
    /// <para>1) All structures are immutable => use <see cref="ReplicationBehavior.Copy"/> behavior.</para>
    /// <para>2) If all members of a given <see cref="Type"/> have <see cref="ReplicationBehavior.Copy"/> behavior =>
    /// type has <see cref="ReplicationBehavior.Copy"/> behavior.</para>
    /// <para>2) All <see cref="Delegate"/>s are not copied during replication.</para>
    /// <para>3) Lazy <see cref="IEnumerable"/> are illegal inside replicating types (causes exception).</para>
    /// </remarks>
    /// <seealso cref="AttributeBasedMetadataProvider"/>
    /// <seealso cref="CustomReplicationBehaviorAttribute"/>
    //todo: test + modify AssertIsNotLazyEnumerable
    public class ConventionalMetadataProvider: AttributeBasedMetadataProvider
    {
        private const string lazyObjectFoundError = @"You should not replicate lazy objects.
If replicating of this type makes sence, then mark it with CustomReplicationBehavior attribute";

        public override ReplicationBehavior GetPerTypeBehavior(Type type)
        {
            ReplicationBehavior behavior;
            if (TryGetDefaultBehavior(type, out behavior))
                return behavior;
            if (TryGetBehaviorFromTypeAttribute(type, out behavior))
                return behavior;

            AssertIsNotLazyEnumerable(type);

            if (typeof (Delegate).IsAssignableFrom(type))
                return ReplicationBehavior.Ignore;
            if (type.IsValueType)
                return ReplicationBehavior.Copy;
            return ReplicationBehavior.Replicate;
        }

        private static void AssertIsNotLazyEnumerable(Type entityType)
        {
            if (entityType.ImplementsGenericInterface(typeof (IEnumerator<>)) || typeof (IEnumerator).IsAssignableFrom(entityType))
                throw new InvalidOperationException(string.Format(
                    "Potential enumerator found: {0}.\n{1}", entityType, lazyObjectFoundError));

            if (entityType.ImplementsGenericInterface(typeof (IEnumerable<>)) || typeof (IEnumerable).IsAssignableFrom(entityType))
                if (!entityType.ImplementsGenericInterface(typeof (ICollection<>))
                    && !typeof (ICollection).IsAssignableFrom(entityType))
                    throw new InvalidOperationException(string.Format(
                        "Potential lazy enumerable found: {0}.\n{1}", entityType, lazyObjectFoundError));
        }
    }
}