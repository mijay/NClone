using System;
using System.Linq;
using NClone.Utils;

namespace NClone.ObjectReplication
{
    /// <summary>
    /// Indicates that object graph, which should be replicated, contains some cycles.
    /// </summary>
    /// <remarks>
    /// Currently <see cref="ObjectReplicator"/> does not support replicating of cyclic object graphs.
    /// </remarks>
    public class CircularReferenceFoundException: Exception
    {
        public CircularReferenceFoundException(object replicatingObject, object[] cycle)
            : base(string.Format(
                //note: when hooks will be added - change this message
                "Reference cycle found while cloning object {0}:\n{1}\n"
                + "Currently copying cyclic object graph is not supported.",
                replicatingObject, cycle.Select(x => x.ToString()).JoinStrings(" -> ")))
        {
        }
    }
}