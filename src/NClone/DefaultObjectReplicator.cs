using NClone.MetadataProviders;
using NClone.ObjectReplication;

namespace NClone
{
    /// <summary>
    /// Class that is able to replicate acyclic object graphs. Static wrapper over <see cref="ObjectReplicator"/>.
    /// </summary>
    /// <remarks>
    /// <para>Replica (or clone, or deep copy) of the given object graph is other object graph, which is semantically equal
    /// to the given one, but not equal to it by-reference. Deep copying can be understood as creating <see cref="object.MemberwiseClone"/>
    /// of the given object and then replacing each object it is referencing by its <see cref="object.MemberwiseClone"/>.</para>
    /// 
    /// <para><see cref="DefaultObjectReplicator"/> is more smart than just recursive call of <see cref="object.MemberwiseClone"/>. First, it is able
    /// to detect double referencing of the same object. For example, when objectA references objectB and objectC, which both references objectD,
    /// then in replicated object graph replicas of objectB and objectC will also reference single object - replica of objectD.</para>
    /// 
    /// <para>Second, <see cref="DefaultObjectReplicator"/> uses few heuristics to guess whether specific type of type member
    /// should be replicated, copied or ignored (then the default value is used in resulting object graph). For example, all <c>struct</c>
    /// are considered as immutable and though only copied. You can read more about used heuristics in <see cref="ConventionalMetadataProvider"/>.</para>
    /// 
    /// <para>Default behavior can be adjusted by using <see cref="CustomReplicationBehaviorAttribute"/> that sets <see cref="ReplicationBehavior"/>
    /// for specific types, fields or auto-properties. You can read more about this mechanism in <see cref="ConventionalMetadataProvider"/>.</para>
    /// 
    /// <para>Note that <see cref="DefaultObjectReplicator"/> is only a static interface to the single instance of <see cref="ObjectReplicator"/>,
    /// which uses <see cref="ConventionalMetadataProvider"/> to get meta-information about replicating types. In case you need more extensibility,
    /// consider using <see cref="ObjectReplicator"/> instead.</para>
    /// </remarks>
    /// <seealso cref="ObjectReplicator"/>
    /// <seealso cref="ConventionalMetadataProvider"/>
    /// <seealso cref="AttributeBasedMetadataProvider"/>
    /// <seealso cref="CustomReplicationBehaviorAttribute"/>
    public static class DefaultObjectReplicator
    {
        private static readonly ObjectReplicator instance = new ObjectReplicator(new ConventionalMetadataProvider());

        /// <summary>
        /// Replicate given <paramref name="source"/> acyclic object graph and return semantically (but not reference) equal graph.
        /// </summary>
        /// <exception cref="CircularReferenceFoundException">
        /// Is thrown when the reference cycle is found in source object graph.
        /// </exception>
        public static T Replicate<T>(T source)
        {
            return instance.Replicate(source);
        }
    }
}