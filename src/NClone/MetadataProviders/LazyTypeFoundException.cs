using System;
using System.Collections.Generic;
using System.Linq;

namespace NClone.MetadataProviders
{
#if NET40
    /// <summary>
    /// Exception indicating that lazy type, e.g. lazily evaluated <see cref="IEnumerable{T}"/>,
    /// or <see cref="IQueryable{T}"/>, or <see cref="Lazy{T}"/>, found inside replicated type.
    /// </summary>
#else
    /// <summary>
    /// Exception indicating that lazy type, e.g. lazily evaluated <see cref="IEnumerable{T}"/>,
    /// or <see cref="IQueryable{T}"/>, found inside replicated type.
    /// </summary>
#endif
    public class LazyTypeFoundException: Exception
    {
        /// <summary>
        /// Constructor for <see cref="LazyTypeFoundException"/>.
        /// </summary>
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