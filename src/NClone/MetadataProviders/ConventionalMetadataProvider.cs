using System;
using System.Collections;
using NClone.Utils;

namespace NClone.MetadataProviders
{
    /// <summary>
    /// Implementation of <see cref="IMetadataProvider"/> that uses conventions
    /// and information from <see cref="CustomReplicationBehaviorAttribute"/>s.
    /// </summary>
    /// <remarks>
    /// <para>Used conventions are:
    /// <list type="bullet">
    /// <item>Structures are copied by-value during cloning.</item>
    /// <item><see cref="Delegate"/>s are not copied during cloning.</item>
    /// <item>Lazy <see cref="IEnumerable"/> inside cloning types causes exception.</item>
    /// </list></para>
    /// 
    /// <para>If you want to override conventional behavior for some type or member, apply
    /// <see cref="CustomReplicationBehaviorAttribute"/> to it.</para>
    /// </remarks>
    /// <seealso cref="AttributeBasedMetadataProvider"/>
    /// <seealso cref="CustomReplicationBehaviorAttribute"/>
    public class ConventionalMetadataProvider: AttributeBasedMetadataProvider
    {
        /// <inheritdoc />
        protected override ReplicationBehavior? TryGetPerTypeBehavior(Type type)
        {
            ReplicationBehavior? result = base.TryGetPerTypeBehavior(type);
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