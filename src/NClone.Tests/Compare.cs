using System;
using System.Diagnostics;
using GeorgeCloney;
using NClone.Annotation;
using NClone.FieldCopying;
using NClone.TypeReplication;
using NUnit.Framework;
using ObjectReplicator = NClone.SecondVersion.ObjectReplicator;

namespace NClone.Tests
{
    [Ignore]
    public class Compare: TestBase
    {
        private class ClassA
        {
            public ClassB field;
        }

        private class ClassB
        {
            public ClassC field;
        }

        private class ClassC: ClassA { }

        private class Class
        {
            public int field;
        }

        [Test]
        public void ComplexCase()
        {
            var source = new ClassC { field = new ClassB { field = new ClassC { field = new ClassB() } } };
            CompareImplementations(source);
        }

        [Test]
        public void SimpleCase()
        {
            var source = new Class { field = RandomInt() };
            CompareImplementations(source);
        }

        [Test]
        public void ImmutableObjectCase()
        {
            var source = DateTime.Now;
            CompareImplementations(source);
        }

        private static void CompareImplementations<T>(T source)
        {
            var timer = new Stopwatch();

            Console.WriteLine("George first run");
            timer.Restart();
            CloneExtension.DeepCloneWithoutSerialization(source);
            timer.Stop();
            Console.Write(timer.ElapsedMilliseconds+"\n");
            Console.Write(timer.ElapsedTicks + "\n");

            Console.WriteLine("George second run");
            timer.Restart();
            CloneExtension.DeepCloneWithoutSerialization(source);
            timer.Stop();
            Console.Write(timer.ElapsedMilliseconds + "\n");
            Console.Write(timer.ElapsedTicks + "\n");

            EntityReplicatorsBuilder entityReplicatorsBuilder = null;
            entityReplicatorsBuilder = new EntityReplicatorsBuilder(new DefaultMetadataProvider(),
                new FieldCopiersBuilder(() => entityReplicatorsBuilder));

            Console.WriteLine("Mine first run");
            timer.Restart();
            entityReplicatorsBuilder.BuildFor(typeof(T)).Replicate(source);
            timer.Stop();
            Console.Write(timer.ElapsedMilliseconds + "\n");
            Console.Write(timer.ElapsedTicks + "\n");

            Console.WriteLine("Mine second run");
            timer.Restart();
            entityReplicatorsBuilder.BuildFor(typeof(T)).Replicate(source);
            timer.Stop();
            Console.Write(timer.ElapsedMilliseconds + "\n");
            Console.Write(timer.ElapsedTicks + "\n");

            Console.WriteLine("Mine v2 first run");
            timer.Restart();
            ObjectReplicator.Replicate(source);
            timer.Stop();
            Console.Write(timer.ElapsedMilliseconds + "\n");
            Console.Write(timer.ElapsedTicks + "\n");

            Console.WriteLine("Mine v2 second run");
            timer.Restart();
            ObjectReplicator.Replicate(source);
            timer.Stop();
            Console.Write(timer.ElapsedMilliseconds + "\n");
            Console.Write(timer.ElapsedTicks + "\n");
        }
    }
}