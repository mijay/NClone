using System;
using JetBrains.Annotations;

namespace NClone.Benchmarks.Runner
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false), MeansImplicitUse]
    public sealed class BenchmarkAttribute: Attribute
    {
    }
}