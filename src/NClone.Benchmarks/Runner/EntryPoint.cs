using System;
using System.Linq;
using System.Reflection;
using BenchmarkDotNet;
using NClone.Utils;

namespace NClone.Benchmarks.Runner
{
    internal static class EntryPoint
    {
        public static void Main()
        {
            BenchmarkSettings.Instance.DefaultPrintBenchmarkBodyToConsole = false;
            BenchmarkSettings.Instance.DetailedMode = true;

            var competition = new BenchmarkCompetition();

            Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => !x.IsAbstract && typeof (CompetitionBase).IsAssignableFrom(x))
                .Select(type => {
                            try {
                                return new { type, instance = Activator.CreateInstance(type) };
                            } catch (Exception e) {
                                Console.WriteLine("Failed: cannot create instance of type {0}:\n{1}", type, e);
                                return null;
                            }
                        })
                .Where(x => x != null)
                .SelectMany(x => x.type
                    .GetMethods(BindingFlags.Public | BindingFlags.NonPublic |
                                BindingFlags.Instance | BindingFlags.Static)
                    .Where(method => method.HasAttribute<BenchmarkAttribute>())
                    .Select(method => new { x.type, x.instance, method }))
                .ForEach(x => {
                             object benchmarkAction = x.method.IsStatic
                                 ? x.method.Invoke(null, new object[0])
                                 : x.method.Invoke(x.instance, new object[0]);
                             competition.AddTask(x.type.Name + "/" + x.method.Name, (Action) benchmarkAction);
                         });

            competition.Run();
        }
    }
}