using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NClone.MetadataProviders;
using NClone.ObjectReplication;

namespace NClone
{
    /// <summary>
    /// Class that is able to clone (aka replicate or deep copy) arbitrary object graphs. Static wrapper over <see cref="ObjectReplicator"/>.
    /// </summary>
    /// <remarks>
    /// <para>Deep copy of the given object graph is other object graph, which is semantically equal (isomorphic) to the given one,
    /// but is not reference equal to it. And hence, if some object in the original object graph is modified, it does not affect cloned 
    /// object graph. Note that object graph topology is preserved during cloning. That is, if in original graph two objects (say objectA
    /// and objectB) reference single object (objectC), then in the cloned graph clone(objectA) and clone(objectB) will reference single
    /// clone(objectC).</para>
    /// 
    /// <para>The way how <see cref="Clone"/> clones objects can be configured. First, it respects annotations done via
    /// <see cref="CustomReplicationBehaviorAttribute"/> (see more in <see cref="AttributeBasedMetadataProvider"/>
    /// and <see cref="CustomReplicationBehaviorAttribute"/>). And second, it follows three basic conventions (see more in
    /// <see cref="ConventionalMetadataProvider"/>):
    /// <list type="bullet">
    /// <item>do not deep copy structures — copies them by-value instead;</item>
    /// <item>do not copy delegates — uses <c>null</c>s in resulting object graphs;</item>
    /// <item>throws when lazy-evaluated (like non-collection <see cref="IEnumerable{T}"/> or <see cref="IQueryable{T}"/>) objects are found.</item>
    /// </list></para>
    /// 
    /// <para>The main method of <see cref="Clone"/> class is <see cref="ObjectGraph{T}"/>, that executes object cloning. In case you do
    /// not want <see cref="Clone"/> to take described conventions into account, you can use <see cref="ObjectIgnoringConventions{T}"/>
    /// method instead.</para>
    /// 
    /// <para>Note that <see cref="Clone"/> is only a static wrapper over an instance of <see cref="ObjectReplicator"/> class,
    /// which uses specific <see cref="IMetadataProvider"/> to get meta-information about replicating types. In case you need
    /// more extensibility, consider using <see cref="ObjectReplicator"/> directly.</para>
    /// </remarks>
    /// <seealso cref="ObjectReplicator"/>
    /// <seealso cref="ConventionalMetadataProvider"/>
    /// <seealso cref="AttributeBasedMetadataProvider"/>
    /// <seealso cref="CustomReplicationBehaviorAttribute"/>
    [PublicAPI]
    public static class Clone
    {
        private static readonly ObjectReplicator conventionalReplicator = new ObjectReplicator(new ConventionalMetadataProvider());
        private static readonly ObjectReplicator unconventionalReplicator = new ObjectReplicator(new AttributeBasedMetadataProvider());

        static Clone()
        {
        }

        /// <summary>
        /// Replicates given <paramref name="source"/> object graph and return its deep copy.
        /// </summary>
        /// <remarks>
        /// Uses both: conventions and attributes to adjust cloning process. Equivalent to <see cref="ObjectReplicator"/>
        /// with <see cref="ConventionalMetadataProvider"/>.
        /// </remarks>
        /// <exception cref="ReplicationException">
        /// Is thrown when object found that cannot be replicated. Examples of such objects are: COM objects, structures on
        /// the last step (when traversed depth-first) of a reference cycle.
        /// </exception>
        [PublicAPI]
        public static T ObjectGraph<T>(T source)
        {
            return conventionalReplicator.Replicate(source);
        }

        /// <summary>
        /// Replicates given <paramref name="source"/> object graph and return its deep copy.
        /// </summary>
        /// <remarks>
        /// Uses only attributes to adjust cloning process. Equivalent to <see cref="ObjectReplicator"/>
        /// with <see cref="AttributeBasedMetadataProvider"/>.
        /// </remarks>
        /// <exception cref="ReplicationException">
        /// Is thrown when object found that cannot be replicated. Examples of such objects are: COM objects, structures on
        /// the last step (when traversed depth-first) of a reference cycle.
        /// </exception>
        [PublicAPI]
        public static T ObjectIgnoringConventions<T>(T source)
        {
            return unconventionalReplicator.Replicate(source);
        }
    }
}