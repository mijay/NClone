using System;

namespace NClone.Tests.Benchmarks
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class BenchmarkAttribute: Attribute { }
}