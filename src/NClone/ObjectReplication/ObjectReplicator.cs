using System;
using mijay.Utils;
using NClone.MetadataProviders;
using NClone.ReplicationStrategies;

namespace NClone.ObjectReplication
{
    /// <summary>
    /// Object that is able to replicate object graphs.
    /// </summary>
    /// <remarks>
    /// <para>Deep copy of the given object graph is other object graph, which is semantically equal (isomorphic) to the given one,
    /// but is not reference equal to it. And hence, if some object in the original object graph is modified, it does not affect cloned 
    /// object graph. Note that object graph topology is preserved during cloning. That is, if in original graph two objects (say objectA
    /// and objectB) reference single object (objectC), then in the cloned graph clone(objectA) and clone(objectB) will reference single
    /// clone(objectC).</para>
    ///  
    /// <para>The way how <see cref="ObjectReplicator"/> replicates objects is configurable. It takes into account
    /// <see cref="ReplicationBehavior"/>s for types and type members provided by <see cref="IMetadataProvider"/>. Depending on
    /// <see cref="ReplicationBehavior"/> objects are either deep copied, by-value copied or ignored (default value is used in resulting
    /// object graph). Note that for each object in a source graph (except top-level object) two <see cref="ReplicationBehavior"/>s are
    /// provided: defined for object type and for member, where object is stored. Actual <see cref="ReplicationBehavior"/> is computed
    /// as a minimum of them.</para>
    /// 
    /// <para><see cref="ObjectReplicator"/> intensively uses caching. Usually second replication of object of the same type
    /// is 5-100 times faster than the first one. Consequently, it is strongly recommended to reuse a single instance of
    /// <see cref="ObjectReplicator"/>. For the most of the cases you can avoid creating instance of <see cref="ObjectReplicator"/>
    /// manually, but to use static API to the single instance via <see cref="Clone"/> class.</para>
    /// </remarks>
    /// <seealso cref="Clone"/>
    /// <seealso cref="IMetadataProvider"/>
    public class ObjectReplicator
    {
        private readonly ReplicationStrategyFactory replicationStrategyFactory;

        /// <summary>
        /// Build instance of <see cref="ObjectReplicator"/>.
        /// </summary>
        public ObjectReplicator(IMetadataProvider metadataProvider)
        {
            Guard.AgainstNull(metadataProvider, "metadataProvider");
            replicationStrategyFactory = new ReplicationStrategyFactory(metadataProvider);
        }

        /// <summary>
        /// Replicate <paramref name="source"/>.
        /// </summary>
        /// <exception cref="ReplicationException">
        /// Is thrown when object found that cannot be replicated. Examples of such objects are: COM objects, structures on
        /// the last step (when traversed depth-first) of a reference cycle.
        /// </exception>
        public T Replicate<T>(T source)
        {
            var result = new ReplicationContext(replicationStrategyFactory).ReplicateAsync(source);
            if (!result.IsCompleted)
                throw new InvalidOperationException("Cannot replicate object graph - the resulting task is not completed!");
            return result.Result.As<T>();
        }
    }
}