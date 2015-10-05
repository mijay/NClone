using BenchmarkDotNet;
using GeorgeCloney;
using NClone.MetadataProviders;
using NClone.ObjectReplication;

namespace NClone.Benchmarks.Competitions
{
    public class SimpleGraphReplicationCompetition
    {
        private SomeClass source;
        private ObjectReplicator replicator;

        [Setup]
        public void Setup()
        {
            source = new SomeClass
            {
                field = 42,
                Property = new SomeClass2 {Property = new SomeClass3 {field = new int[1240]}}
            };

            replicator = new ObjectReplicator(new ConventionalMetadataProvider());
        }

        [Benchmark]
        public void ReplicatorClone()
        {
            source = replicator.Replicate(source);
        }

        [Benchmark]
        public void ReflectionClone()
        {
            source = source.DeepClone();
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