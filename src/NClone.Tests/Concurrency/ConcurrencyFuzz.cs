using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NClone.Tests.Concurrency
{
    public class ConcurrencyFuzz : TestBase
    {
        [Test]
        public void ConcurrentClones_NoException()
        {
            Assert.DoesNotThrow(
                () => Parallel.ForEach(Enumerable.Repeat<Func<ClonesOnInstantiate>>(() => new ClonesOnInstantiate(), 20), x => x())
            );
        }

        public class ClonesOnInstantiate
        {
            private ClonedClass _foo1 = NClone.Clone.ObjectGraph(ClonedClass.Empty);
            public ClonedClass Foo1
            {
                get { return _foo1; }
                set { _foo1 = value; }
            }

            private ClonedClass _foo3 = NClone.Clone.ObjectGraph(ClonedClass.Empty);
            public ClonedClass Foo3
            {
                get { return _foo3; }
                set { _foo3 = value; }
            }
        }

        public class ClonedClass
        {
            private readonly int _blah;

            private ClonedClass() : this(Int32.MinValue) { }

            public ClonedClass(int blah)
            {
                _blah = blah;
            }

            public static readonly ClonedClass Empty = new ClonedClass();
        }
    }
}
