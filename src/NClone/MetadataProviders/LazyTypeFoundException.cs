using System;

namespace NClone.MetadataProviders
{
    public class LazyTypeFoundException: Exception
    {
        public LazyTypeFoundException(string message): base(message) { }
    }
}