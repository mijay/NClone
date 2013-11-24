using System;
using System.Linq;
using FakeItEasy;
using FakeItEasy.ExtensionSyntax;
using mijay.Utils;
using mijay.Utils.Collections;
using NClone.ObjectReplication;
using NClone.ReplicationStrategies;
using NClone.Utils;
using NUnit.Framework;

namespace NClone.Tests.ReplicationStrategies
{
    public class CloneArrayReplicationStrategyTest: TestBase
    {
        private CloneArrayReplicationStrategy replicator;
        private IReplicationContext dummyContext;

        protected override void SetUp()
        {
            base.SetUp();
            replicator = new CloneArrayReplicationStrategy(typeof(Class));
            dummyContext = A.Fake<IReplicationContext>(x => x.Strict());
        }

        private static IReplicationContext ContextThatMaps(Class[] source, Class[] to)
        {
            var context = A.Fake<IReplicationContext>(x => x.Strict());
            source
                .Zip(to, Tuple.Create)
                .ForEach(pair => context
                    .Configure()
                    .CallsTo(x => x.Replicate(pair.Item1))
                    .Returns(pair.Item2));
            return context;
        }

        [Test]
        public void SourceIsEmptyArray_EmptyArrayReturned()
        {
            var source = new Class[0];

            object result = replicator.Replicate(source, dummyContext);

            Assert.That(result.As<Class[]>(), Is.Empty);
        }

        [Test]
        public void SourceIsArrayWithElements_EachElementIsReplicated()
        {
            var source = new[] { new Class(), new Class() };
            var expectedResult = new[] { new Class(), new Class() };

            object result = replicator.Replicate(source, ContextThatMaps(source, to: expectedResult));

            Assert.That(result.As<Class[]>(), Is.EquivalentTo(expectedResult));
            CollectionAssert.AreEqual(expectedResult, result.As<Class[]>());
        }

        [Test]
        public void SourceIsArrayOfElementsOfOtherType_ExceptionIsThrown()
        {
            TestDelegate action = () => replicator.Replicate(new[] { new Inherited() }, dummyContext);

            Assert.That(action, Throws.InstanceOf<ArgumentException>());
        }

        private class Class
        {
        }

        private class Inherited: Class
        {
        }
    }
}