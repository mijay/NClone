using System;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using FakeItEasy.ExtensionSyntax;
using FakeItEasy.ExtensionSyntax.Full;
using mijay.Utils;
using mijay.Utils.Collections;
using NClone.ObjectReplication;
using NClone.ReplicationStrategies;
using NUnit.Framework;

namespace NClone.Tests.ReplicationStrategies
{
    public class CloneArrayReplicationStrategyTest: TestBase
    {
        private IReplicationContext dummyContext;
        private CloneArrayReplicationStrategy replicator;

        protected override void SetUp()
        {
            base.SetUp();
            replicator = new CloneArrayReplicationStrategy(typeof (Class));
            dummyContext = A.Fake<IReplicationContext>(x => x.Strict());
        }

        private static IReplicationContext ContextThatMaps<T>(T[] source, T[] to)
        {
            var context = A.Fake<IReplicationContext>(x => x.Strict());
            source
                .Zip(to, Tuple.Create)
                .ForEach(pair => context
                    .Configure()
                    .CallsTo(x => x.ReplicateAsync(pair.Item1))
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

            object result = replicator.Replicate(source, ContextThatMaps(source, expectedResult));

            CollectionAssert.AreEqual(expectedResult, result.As<Class[]>());
        }

        [Test]
        public void SourceIsArrayOfElementsOfOtherType_ExceptionIsThrown()
        {
            TestDelegate action = () => replicator.Replicate(new[] { new Inherited() }, dummyContext);

            Assert.That(action, Throws.InstanceOf<ArgumentException>());
        }

        [Test]
        public void SourceIsArrayOfStructs_EachElementIsReplicated()
        {
            replicator = new CloneArrayReplicationStrategy(typeof(Struct));
            var source = new[] { new Struct(), new Struct() };
            var expectedResult = new[] { new Struct(), new Struct() };

            object result = replicator.Replicate(source, ContextThatMaps(source, expectedResult));

            CollectionAssert.AreEqual(expectedResult, result.As<Struct[]>());
        }

        [Test]
        public void SourceIsArrayWithElementsThatAreReplicatedAsync_EachElementIsSetAfterReplication()
        {
            var sourceElement = new Class();
            var resultTaskSource = new TaskCompletionSource<object>();

            var context = A.Fake<IReplicationContext>(x => x.Strict());
            context
                .CallsTo(x => x.ReplicateAsync(sourceElement))
                .ReturnsLazily(_ => resultTaskSource.Task);

            var result = replicator.Replicate(new[] { sourceElement }, context).As<Class[]>();

            CollectionAssert.AreEqual(new Class[] { null }, result);

            var resultElement = new Class();
            resultTaskSource.SetResult(resultElement);
            CollectionAssert.AreEqual(new[] { resultElement }, result);
        }

        private class Class
        {
        }

        private class Inherited: Class
        {
        }

        private struct Struct
        {
        }
    }
}