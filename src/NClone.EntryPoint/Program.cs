using System;
using System.Diagnostics;
using NClone.Annotation;
using NClone.FieldCopying;
using NClone.TypeReplication;

namespace NClone.EntryPoint
{
    internal class Program
    {
        private class Class
        {
            public int field;
        }

        public static void Main(string[] args)
        {
            var source = new Class { field = 42 };

            EntityReplicatorsBuilder entityReplicatorsBuilder = null;
            entityReplicatorsBuilder = new EntityReplicatorsBuilder(new DefaultMetadataProvider(),
                new FieldCopiersBuilder(() => entityReplicatorsBuilder));

            var timer = new Stopwatch();
            timer.Restart();
            var entityReplicator = entityReplicatorsBuilder.BuildFor(typeof (Class));
            entityReplicator.Replicate(source);
            timer.Stop();
            Console.WriteLine(timer.ElapsedTicks);
            timer.Restart();
            entityReplicator.Replicate(source);
            timer.Stop();
            Console.WriteLine(timer.ElapsedTicks);
        }
    }
}