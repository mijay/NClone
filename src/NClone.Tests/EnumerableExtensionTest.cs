using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NClone.Shared;
using NUnit.Framework;

namespace NClone.Tests
{
    public class EnumerableExtensionTest: TestBase
    {
        [Test]
        public void MaterializeCollection_GetSameCollection()
        {
            var source = new Collection<int>() { 1, 2, 3 };

            var result = source.Materialize();

            Assert.That(result, Is.SameAs(source));
        }

        [Test]
        public void MaterializeArray_GetSameArray()
        {
            var source = new[] { 1, 2, 3 };

            var result = source.Materialize();

            Assert.That(result, Is.SameAs(source));
        }

        private class ReadOnlyList<T>: IReadOnlyList<T>
        {
            private readonly T[] array;

            public ReadOnlyList(T[] array)
            {
                this.array = array;
            }

            public IEnumerator<T> GetEnumerator()
            {
                return array.AsEnumerable().GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public int Count
            {
                get { return array.Length; }
            }

            public T this[int index]
            {
                get { return array[index]; }
            }
        }

        [Test]
        public void MaterializeReadOnlyList_GetSameObject()
        {
            IReadOnlyList<int> source = new ReadOnlyList<int>(new[] { 1, 2, 3 });

            var result = source.Materialize();

            Assert.That(result, Is.SameAs(source));
        }
    }
}