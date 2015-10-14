using System;

namespace NClone.ObjectReplication
{
    /// <summary>
    /// Indicates that the object graph cannot be replicated, because it contains objects that aren't supported.
    /// </summary>
    /// <remarks>
    /// Currently replication of the following objects are not supported:
    /// <list type="bullet">
    /// <item>COM objects;</item>
    /// <item><see cref="Type.IsContextful"/> objects;</item>
    /// <item>value-typed objects on the last step (when traversed depth-first) of a reference cycle.</item>
    /// </list>
    /// </remarks>
    public class ReplicationException: Exception
    {
        private ReplicationException(string reason): base("Object graph cannot be replicated: " + reason) { }

        internal static ReplicationException CycleThroughValueTypeFound()
        {
            return new ReplicationException("it contains reference cycle with the last (when traversed depth-first) step being a value type.");
        }
    }
}
