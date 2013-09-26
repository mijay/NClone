using NClone.MetadataProviders;
using NClone.ReplicationStrategies;
using NClone.Shared;

namespace NClone.ObjectReplication
{
    /// <summary>
    /// Object that is able to replicate acyclic object graphs.
    /// </summary>
    /// <remarks>
    /// <para>Replica (or clone, or deep copy) of the given object graph is other object graph, which is semantically equal
    /// to the given one, but not equal to it by-reference. Deep copying can be understood as creating <see cref="object.MemberwiseClone"/>
    /// of the given object and then replacing each object it is referencing by its <see cref="object.MemberwiseClone"/>.</para>
    /// 
    /// <para><see cref="ObjectReplicator"/> is more smart than just recursive call of <see cref="object.MemberwiseClone"/>. First, it is able
    /// to detect double referencing of the same object. For example, when objectA references objectB and objectC, which both references objectD,
    /// then in replicated object graph replicas of objectB and objectC will also reference single object - replica of objectD.</para>
    /// 
    /// <para>Second, <see cref="ObjectReplicator"/> is extensible. It takes into account <see cref="ReplicationBehavior"/>s for types
    /// and type members it is replicating. Depending on <see cref="ReplicationBehavior"/> objects are either ignored
    /// (default value used in resulting object graph), copied to resulted graph or recursively replicated. <see cref="IMetadataProvider"/>
    /// is used to determine which <see cref="ReplicationBehavior"/> to use for type and/or member.</para>
    /// 
    /// <para><see cref="ObjectReplicator"/> intensively uses caching. Usually second replication of object of the same type
    /// is 5-100 times faster than the first one. Consequently, it is strongly recommended to reuse single instance of
    /// <see cref="ObjectReplicator"/>. For the most of the cases you can avoid creating instance of <see cref="ObjectReplicator"/>
    /// manually, but ot use static API to default instance via <see cref="DefaultObjectReplicator"/>.</para>
    /// </remarks>
    /// <seealso cref="DefaultObjectReplicator"/>
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
        /// <exception cref="CircularReferenceFoundException">
        /// Is thrown when the reference cycle is found in source object graph.
        /// </exception>
        public T Replicate<T>(T source)
        {
            return new ReplicationContext(replicationStrategyFactory).Replicate(source).As<T>();
        }
    }
}