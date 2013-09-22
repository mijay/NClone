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
    /// <para>1) All structures are immutable => use <see cref="ReplicationBehavior.Copy"/>.</para>
    /// <para>2) All <see cref="Delegate"/>s are not copied during replication.</para>
    /// <para>3) Lazy <see cref="IEnumerable"/> are illegal inside replicating types (causes exception).</para>
    /// </remarks>
    //todo: test + modify AssertIsNotLazyEnumerable
    public class ConventionalMetadataProvider: AttributeBasedMetadataProvider
    {
        private const string lazyObjectFoundError = @"You should not replicate lazy objects.
If replicating of this type makes sence, then mark it with CustomReplicationBehavior attribute";

        public override ReplicationBehavior GetBehavior(Type entityType)
        {
            ReplicationBehavior behavior;
            if (TryGetDefaultBehavior(entityType, out behavior))
                return behavior;
            if (TryGetBehaviorFromTypeAttribute(entityType, out behavior))
                return behavior;

            AssertIsNotLazyEnumerable(entityType);

            if (typeof (Delegate).IsAssignableFrom(entityType))
                return ReplicationBehavior.Ignore;
            if (entityType.IsValueType)
                return ReplicationBehavior.Copy;
            return ReplicationBehavior.DeepCopy;
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