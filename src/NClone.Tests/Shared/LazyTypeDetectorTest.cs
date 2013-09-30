using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using NClone.Shared;
using NUnit.Framework;

namespace NClone.Tests.Shared
{
    public class LazyTypeDetectorTest: TestBase
    {
        [Test]
        public void EnumerableReturnedByWhere_IsLazy()
        {
            Type type = new[] { 1 }.Where(x => x > 0).GetType();
            Assert.That(LazyTypeDetector.IsLazyType(type), Is.True);
        }

        [Test]
        public void Queryable_IsLazy()
        {
            Assert.That(LazyTypeDetector.IsLazyType(typeof (SomeQueryable<int>)), Is.True);
        }

        [Test]
        public void Lazy_IsLazy()
        {
            Assert.That(LazyTypeDetector.IsLazyType(typeof (Lazy<int>)), Is.True);
        }

        [Test]
        public void Enumerator_IsLazy()
        {
            Type type = new SomeCollection().GetEnumerator().GetType();
            Assert.That(LazyTypeDetector.IsLazyType(type), Is.True);
        }

        [Test]
        public void Collection_IsNotLazy()
        {
            Assert.That(LazyTypeDetector.IsLazyType(typeof(Collection<int>)), Is.False);
        }

        [Test]
        public void ReadonlyCollection_IsNotLazy()
        {
            Assert.That(LazyTypeDetector.IsLazyType(typeof(ReadOnlyCollection<int>)), Is.False);
        }

        [Test]
        public void UserCollection_IsNotLazy()
        {
            Assert.That(LazyTypeDetector.IsLazyType(typeof(SomeCollection)), Is.False);
        }

        private class SomeCollection: IEnumerable<int>
        {
            public IEnumerator<int> GetEnumerator()
            {
                for (int i = 0; i < 100; ++i)
                    yield return i;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private class SomeQueryable<T>: List<T>, IQueryable
        {
            public Expression Expression { get; private set; }
            public Type ElementType { get; private set; }
            public IQueryProvider Provider { get; private set; }
        }
    }
}