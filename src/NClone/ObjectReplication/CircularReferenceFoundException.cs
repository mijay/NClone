using System;

namespace NClone.ObjectReplication
{
    // todo: proper documentation here, in Clone, and in ObjectReplicator
    public class CircularReferenceFoundException: Exception
    {
       public CircularReferenceFoundException()
       {
       }
    }
}