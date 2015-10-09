using System.Threading.Tasks;

namespace NClone.ObjectReplication
{
    /// <summary>
    /// Represents the context of replication of a single object graph. It tracks cloned objects to preserve
    /// object graph topology in cloned graph (e.g. when one object is referenced twice in source object graph,
    /// it should also be reference twice in resulting graph).
    /// </summary>
    internal interface IReplicationContext
    {
        /// <summary>
        /// Replicates <paramref name="source"/> or gets its cached replica. In case when <paramref name="source"/>
        /// is part of the cycle in an object graph, it is impossible to compute it immediately. To accommodate
        /// this case, <see cref="ReplicateAsync"/> always returns its result wrapped in <see cref="Task{TResult}"/>.
        /// </summary>
        Task<object> ReplicateAsync(object source);
    }
}