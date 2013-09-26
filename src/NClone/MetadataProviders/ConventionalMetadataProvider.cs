using System;
using System.Collections;
using System.Linq;
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
    /// <para>2) All <see cref="Delegate"/>s are not copied during replication.</para>
    /// <para>3) Lazy <see cref="IEnumerable"/> are illegal inside replicating types (causes exception).</para>
    /// </remarks>
    /// <seealso cref="AttributeBasedMetadataProvider"/>
    /// <seealso cref="CustomReplicationBehaviorAttribute"/>
    public class ConventionalMetadataProvider: AttributeBasedMetadataProvider
    {
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

        private static void AssertIsNotLazyEnumerable(Type type)
        {
            if (LazyTypeDetector.IsLazyEnumerable(type))
                throw new InvalidOperationException(string.Format(
                    @"Potential enumerator found: {0}.
You should not replicate lazy objects.
If replicating of this type makes sense, then mark it with CustomReplicationBehavior attribute", type.FullName));
        }
    }
}