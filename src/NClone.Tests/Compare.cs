using System;
using System.Diagnostics;
using GeorgeCloney;
using NClone.Annotation;
using NClone.FieldCopying;
using NClone.TypeReplication;
using NUnit.Framework;

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

            Debug.WriteLine("George first run");
            timer.Restart();
            CloneExtension.DeepCloneWithoutSerialization(source);
            timer.Stop();
            Debug.WriteLine(timer.ElapsedMilliseconds);
            Debug.WriteLine(timer.ElapsedTicks);

            Debug.WriteLine("George second run");
            timer.Restart();
            CloneExtension.DeepCloneWithoutSerialization(source);
            timer.Stop();
            Debug.WriteLine(timer.ElapsedMilliseconds);
            Debug.WriteLine(timer.ElapsedTicks);

            EntityReplicatorsBuilder entityReplicatorsBuilder = null;
            entityReplicatorsBuilder = new EntityReplicatorsBuilder(new DefaultMetadataProvider(),
                new FieldCopiersBuilder(() => entityReplicatorsBuilder));

            Debug.WriteLine("Mine first run");
            timer.Restart();
            entityReplicatorsBuilder.BuildFor(typeof(T)).Replicate(source);
            timer.Stop();
            Debug.WriteLine(timer.ElapsedMilliseconds);
            Debug.WriteLine(timer.ElapsedTicks);

            Debug.WriteLine("Mine second run");
            timer.Restart();
            entityReplicatorsBuilder.BuildFor(typeof(T)).Replicate(source);
            timer.Stop();
            Debug.WriteLine(timer.ElapsedMilliseconds);
            Debug.WriteLine(timer.ElapsedTicks);
        }
    }
}