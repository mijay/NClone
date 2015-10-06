using BenchmarkDotNet;
using BenchmarkDotNet.Tasks;
using GeorgeCloney;
using NClone.MetadataProviders;
using NClone.ObjectReplication;

namespace NClone.Benchmarks.Competitions
{
    [BenchmarkTask(platform: BenchmarkPlatform.X86, framework: BenchmarkFramework.V35)]
    [BenchmarkTask(platform: BenchmarkPlatform.X86, framework: BenchmarkFramework.V40)]
    [BenchmarkTask(platform: BenchmarkPlatform.X86, framework: BenchmarkFramework.V45)]
    [BenchmarkTask(platform: BenchmarkPlatform.X64, framework: BenchmarkFramework.V40)]
    [BenchmarkTask(platform: BenchmarkPlatform.X64, framework: BenchmarkFramework.V45)]
    [BenchmarkTask(platform: BenchmarkPlatform.X64, framework: BenchmarkFramework.V451)]
    [BenchmarkTask(platform: BenchmarkPlatform.X64, framework: BenchmarkFramework.V452)]
    public class SimpleGraphReplicationCompetition
    {
        SomeClass source;
        ObjectReplicator replicator;

        [Setup]
        public void Setup()
        {
            source = new SomeClass
            {
                field = 42,
                Property = new SomeClass2 { Property = new SomeClass3 { field = new int[1240] } }
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

        class SomeClass
        {
            public int field;
            public SomeClass2 Property { get; set; }
        }

        class SomeClass2
        {
            public SomeClass3 Property { get; set; }
        }

        class SomeClass3
        {
            public int[] field;
        }
    }
}
