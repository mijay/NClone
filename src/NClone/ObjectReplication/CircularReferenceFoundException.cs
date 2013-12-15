using System;

namespace NClone.ObjectReplication
{
    /// <summary>
    /// Indicates that object graph, which should be replicated, contains a cycle.
    /// </summary>
    public class CircularReferenceFoundException: Exception
    {
        /// <summary>
        /// Default constructor for <see cref="CircularReferenceFoundException"/>.
        /// </summary>
        public CircularReferenceFoundException()
            : base(
                //note: when hooks will be added - change this message
                "Cyclic reference found during replication. Currently copying cyclic object graph is not supported.")
        {
        }
    }
}