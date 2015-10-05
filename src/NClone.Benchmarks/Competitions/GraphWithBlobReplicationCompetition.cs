using System.Linq;
using BenchmarkDotNet;
using GeorgeCloney;
using NClone.MetadataProviders;
using NClone.ObjectReplication;

namespace NClone.Benchmarks.Competitions
{
    public class GraphWithBlobReplicationCompetition
    {
        private const int blobSize = 200000;
        private SomeClass source;
        private ObjectReplicator replicator;

        [Setup]
        public void Setup()
        {
            source = CreateData();
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

        [Benchmark]
        public void SimpleDataCreation()
        {
            CreateData();
        }

        private static SomeClass CreateData()
        {
            return new SomeClass
            {
                Property = Enumerable.Range(0, blobSize)
                    .Select(i => new SomeClass2 {field = new SomeClass3 {field = i.ToString()}})
                    .ToArray()
            };
        }

        private class SomeClass
        {
            public SomeClass2[] Property { get; set; }
        }

        private class SomeClass2
        {
            public SomeClass3 field;
        }

        private class SomeClass3
        {
            public string field;
        }
    }
}