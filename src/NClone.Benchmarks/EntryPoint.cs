using System.IO;
using BenchmarkDotNet;
using BenchmarkDotNet.Logging;
using NClone.Benchmarks.Competitions;

namespace NClone.Benchmarks
{
    static class EntryPoint
    {
        public static void Main()
        {
            using (var writer = new StreamWriter("benchmark.log"))
            {
                var runner = new BenchmarkRunner(new IBenchmarkLogger[] { new BenchmarkStreamLogger(writer), new BenchmarkConsoleLogger() });
                //runner.RunCompetition(new ArrayAccessCompetition());
                //runner.RunCompetition(new DictionaryCompetition());
                //runner.RunCompetition(new SetFieldCompetition());
                //runner.RunCompetition(new ArrayCreationCompetition());
                runner.RunCompetition(new GraphWithBlobReplicationCompetition());
                runner.RunCompetition(new SimpleGraphReplicationCompetition());
            }
        }
    }
}
