using System;

namespace NClone.ObjectReplication
{
    /// <summary>
    /// Indicates that object graph, which should be replicated, contains a cycle.
    /// </summary>
    [Obsolete("This exception is never thrown in NClone 1.5+, since replicating of recursive structures is supported")]
    public class CircularReferenceFoundException: Exception
    {
       private CircularReferenceFoundException()
       {
       }
    }
}