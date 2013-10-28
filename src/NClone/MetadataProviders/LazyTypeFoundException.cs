using System;

namespace NClone.MetadataProviders
{
    public class LazyTypeFoundException: Exception
    {
        public LazyTypeFoundException(Type type)
            : base(string.Format(
                "Lazy type found: {0}.\n"
                + "You should not replicate lazy types. But if replicating of this type makes sense, "
                + "then mark it (or field that contains it) with CustomReplicationBehavior attribute.",
                type.FullName))
        {
        }
    }
}