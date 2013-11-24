using System;
using GeorgeCloney;
using NClone.Benchmarks.Runner;
using NClone.MetadataProviders;
using NClone.ObjectReplication;

namespace NClone.Benchmarks
{
    public class SimpleGraphReplicationCompetition: CompetitionBase
    {
        private const int iterationCount = 200000;

        [Benchmark]
        public Action OneRun()
        {
            var source = new SomeClass
                         {
                             field = 42,
                             Property = new SomeClass2 { Property = new SomeClass3 { field = new int[1240] } }
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
                             Property = new SomeClass2 { Property = new SomeClass3 { field = new int[1240] } }
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
                             Property = new SomeClass2 { Property = new SomeClass3 { field = new int[1240] } }
                         };

            return () => {
                       for (int i = 0; i < iterationCount; ++i)
                           source = source.DeepClone();
                   };
        }

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
            public int[] field;
        }
    }
}