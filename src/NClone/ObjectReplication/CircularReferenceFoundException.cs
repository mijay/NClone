using System;

namespace NClone.ObjectReplication
{
    /// <summary>
    /// Indicates that object graph which should be replicated contains some cycles.
    /// </summary>
    /// <remarks>
    /// Currently <see cref="ObjectReplicator"/> does not support replicating of cyclic object graphs.
    /// </remarks>
    public class CircularReferenceFoundException: Exception { }
}