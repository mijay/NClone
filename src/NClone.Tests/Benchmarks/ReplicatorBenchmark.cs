using System;
using GeorgeCloney;
using NClone.MetadataProviders;
using NClone.ObjectReplication;

namespace NClone.Tests.Benchmarks
{
    public class ReplicatorBenchmark: BenchmarkBase
    {
        private const int iterationCount = 200000;

        private class SomeClass
        {
            public int field;
            public SomeClass2 Property { get; set; }
        }

        private class SomeClass2
        {
            public SomeClass3 Property { get; set; }
        }

        private class SomeClass3
        {
            public int field;
        }

        [Benchmark]
        public Action OneRun()
        {
            var source = new SomeClass
                         {
                             field = 42,
                             Property = new SomeClass2 { Property = new SomeClass3 { field = 12 } }
                         };

            var replicator = new ObjectReplicator(new ConventionalMetadataProvider());
            replicator.Replicate("12");

            return () => replicator.Replicate(source);
        }

        [Benchmark]
        public Action ManyRuns()
        {
            var source = new SomeClass
                         {
                             field = 42,
                             Property = new SomeClass2 { Property = new SomeClass3 { field = 12 } }
                         };

            var replicator = new ObjectReplicator(new ConventionalMetadataProvider());

            return () => {
                       for (int i = 0; i < iterationCount; ++i)
                           source = replicator.Replicate(source);
                   };
        }

        [Benchmark]
        public Action ReflectionClone()
        {
            var source = new SomeClass
                         {
                             field = 42,
                             Property = new SomeClass2 { Property = new SomeClass3 { field = 12 } }
                         };

            return () => {
                       for (int i = 0; i < iterationCount; ++i)
                           source = source.DeepClone();
                   };
        }
    }
}