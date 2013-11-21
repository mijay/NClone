using System;
using System.IO;
using System.Linq;
using System.Text;
using BenchmarkDotNet;
using NClone.Utils;
using NUnit.Framework;

namespace NClone.Tests.Benchmarks
{
    [TestFixture, Ignore]
    public class BenchmarkBase
    {
        private class HackTextWriter: TextWriter
        {
            private readonly TextWriter textWriter;

            public HackTextWriter(TextWriter textWriter)
            {
                this.textWriter = textWriter;
            }

            public override Encoding Encoding
            {
                get { return textWriter.Encoding; }
            }

            public override void Write(char value)
            {
                switch (value) {
                    case '\r':
                        return;
                    case '\n':
                        textWriter.WriteLine(char.ConvertFromUtf32(8203));
                        return;
                    case ' ':
                        textWriter.Write(" \u200b");
                        return;
                    default:
                        textWriter.Write(value);
                        return;
                }
            }
        }

        static BenchmarkBase()
        {
            BenchmarkSettings.Instance.DetailedMode = true;
            BenchmarkSettings.Instance.DefaultPrintBenchmarkBodyToConsole = true;
            BenchmarkSettings.Instance.DefaultMaxWarmUpIterationCount = 200;
            BenchmarkSettings.Instance.DefaultWarmUpIterationCount = 20;
            BenchmarkSettings.Instance.DefaultMaxWarmUpError = 0.3;

            Console.SetOut(new HackTextWriter(Console.Error));
        }

        [Test]
        public void BenchmarkEntryPoint()
        {
            Type type = GetType();
            string benchmarkName = type.Name.EndsWith("benchmark", StringComparison.InvariantCultureIgnoreCase)
                ? type.Name.Substring(0, type.Name.Length - "benchmark".Length)
                : type.Name;

            var competition = new BenchmarkCompetition();
            type
                .GetMethods()
                .Where(x => x.HasAttribute<BenchmarkAttribute>())
                .ForEach(buildBenchmarkMethod => {
                             var benchmarkDelegate = buildBenchmarkMethod.Invoke(this, new object[0]).As<Action>();
                             competition.AddTask(benchmarkName + " - " + buildBenchmarkMethod.Name, benchmarkDelegate);
                         });

            competition.Run();
        }
    }
}