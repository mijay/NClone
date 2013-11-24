using System;
using System.Linq;
using NClone.Utils;

namespace NClone.ObjectReplication
{
    /// <summary>
    /// Indicates that object graph, which should be replicated, contains a cycle.
    /// </summary>
    public class CircularReferenceFoundException: Exception
    {
        public CircularReferenceFoundException(object[] cyclePrefix, object[] cycle)
            : base(string.Format(
                //note: when hooks will be added - change this message
                "The following reference cycle found during replication (cycle part is in parentheses):\n{0}({1})\n"
                + "Currently copying cyclic object graph is not supported.",
                cyclePrefix.Any() ? cyclePrefix.Select(x => x.ToString()).JoinStrings(" -> ") + " -> " : "",
                cycle.Select(x => x.ToString()).JoinStrings(" -> ")))
        {
        }
    }
}