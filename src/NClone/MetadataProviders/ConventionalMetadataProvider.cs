using System;
using System.Collections;
using NClone.Utils;

namespace NClone.MetadataProviders
{
    /// <summary>
    /// Implementation of <see cref="IMetadataProvider"/> that is based on conventions
    /// and information from <see cref="CustomReplicationBehaviorAttribute"/>s.
    /// </summary>
    /// <remarks>
    /// <para>Used conventions:
    /// <list type="bullet">
    /// <item>All structures are immutable => use <see cref="ReplicationBehavior.Copy"/> behavior.</item>
    /// <item>All <see cref="Delegate"/>s are not copied during replication.</item>
    /// <item>Lazy <see cref="IEnumerable"/> are illegal inside replicating types (causes exception).</item>
    /// </list>
    /// </para>
    /// 
    /// <para>If you want to override conventional behavior for some type or member, just apply
    /// <see cref="CustomReplicationBehaviorAttribute"/> to it.</para>
    /// </remarks>
    /// <seealso cref="AttributeBasedMetadataProvider"/>
    /// <seealso cref="CustomReplicationBehaviorAttribute"/>
    public class ConventionalMetadataProvider: AttributeBasedMetadataProvider
    {
        protected override ReplicationBehavior? TryGetPerTypeBehavior(Type type)
        {
            var result = base.TryGetPerTypeBehavior(type);
            if (result.HasValue)
                return result.Value;

            AssertIsNotLazyEnumerable(type);

            if (typeof (Delegate).IsAssignableFrom(type))
                return ReplicationBehavior.Ignore;
            if (type.IsValueType)
                return ReplicationBehavior.Copy;
            return null;
        }

        private static void AssertIsNotLazyEnumerable(Type type)
        {
            if (LazyTypeDetector.IsLazyType(type))
                throw new LazyTypeFoundException(type);
        }
    }
}