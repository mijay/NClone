using System;
using BenchmarkDotNet;
using BenchmarkDotNet.Tasks;
using mijay.Utils;

namespace NClone.Benchmarks.Competitions
{
    [BenchmarkTask(platform: BenchmarkPlatform.X86, framework: BenchmarkFramework.V35, jitVersion: BenchmarkJitVersion.LegacyJit)]
    [BenchmarkTask(platform: BenchmarkPlatform.X86, framework: BenchmarkFramework.V40, jitVersion: BenchmarkJitVersion.LegacyJit)]
    [BenchmarkTask(platform: BenchmarkPlatform.X86, framework: BenchmarkFramework.V45, jitVersion: BenchmarkJitVersion.LegacyJit)]
    [BenchmarkTask(platform: BenchmarkPlatform.X86, framework: BenchmarkFramework.V46, jitVersion: BenchmarkJitVersion.RyuJit)]
    [BenchmarkTask(platform: BenchmarkPlatform.X64, framework: BenchmarkFramework.V40, jitVersion: BenchmarkJitVersion.LegacyJit)]
    [BenchmarkTask(platform: BenchmarkPlatform.X64, framework: BenchmarkFramework.V45, jitVersion: BenchmarkJitVersion.LegacyJit)]
    [BenchmarkTask(platform: BenchmarkPlatform.X64, framework: BenchmarkFramework.V46, jitVersion: BenchmarkJitVersion.RyuJit)]
    public class ArrayCreationCompetition
    {
        private readonly int[] source = new int[10000];
        private static readonly Type elementType = typeof (int);

        private void Consume(Array array)
        {
            if (array.Length != 10000)
                throw new InvalidOperationException();
        }

        [Benchmark]
        public void Simple()
        {
            var array = (Array) new int[10000];
            Consume(array);
        }

        [Benchmark]
        public void Clone()
        {
            var array = source.As<Array>().Clone().As<Array>();
            Consume(array);
        }

        [Benchmark]
        public void ArrayCreate()
        {
            var array = Array.CreateInstance(elementType, source.Length);
            Consume(array);
        }
    }
}