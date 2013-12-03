using System;
using System.Linq;
using GeorgeCloney;
using NClone.Benchmarks.Runner;
using NClone.MetadataProviders;
using NClone.ObjectReplication;

namespace NClone.Benchmarks
{
    public class GraphWithBlobReplicationCompetition: CompetitionBase
    {
        private const int blobSize = 200000;

        private static void Consume(SomeClass source)
        {
            source.Property.Sum(x => x.field.Length);
        }

        [Benchmark]
        public Action ReplicatorClone()
        {
            SomeClass source = CreateData();

            var replicator = new ObjectReplicator(new ConventionalMetadataProvider());
            replicator.Replicate(new SomeClass());
            replicator.Replicate(new SomeClass2());

            return () => {
                       source = replicator.Replicate(source);
                       Consume(source);
                   };
        }

        [Benchmark]
        public Action ReflectionClone()
        {
            SomeClass source = CreateData();

            return () => {
                       source = source.DeepClone();
                       Consume(source);
                   };
        }

        [Benchmark]
        public Action SimpleDataCreation()
        {
            return () => {
                       SomeClass data = CreateData();
                       Consume(data);
                   };
        }

        private static SomeClass CreateData()
        {
            var source = new SomeClass
                         {
                             Property = Enumerable.Range(0, blobSize)
                                 .Select(i => new SomeClass2 { field = i.ToString() })
                                 .ToArray()
                         };
            return source;
        }

        private class SomeClass
        {
            public SomeClass2[] Property { get; set; }
        }

        private class SomeClass2
        {
            public string field;
        }
    }
}