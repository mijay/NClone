using System;
using System.Linq;
using NClone.MemberAccess;
using NUnit.Framework;

namespace NClone.Tests.MemberAccess
{
    public class ArrayAccessorBuilderTest: TestBase
    {
        private double[] array;
        private double number;

        protected override void SetUp()
        {
            base.SetUp();
            var random = new Random();

            array = Enumerable.Range(0, 100)
                .Select(_ => random.NextDouble())
                .ToArray();
            number = random.NextDouble();
        }

        [Test]
        public void BuildArrayElementReader_ItWorks()
        {
            Func<Array, int, object> arrayElementReader = ArrayAccessorBuilder.BuildArrayElementReader(typeof (double));

            object result = arrayElementReader(array, 25);

            Assert.That(result, Is.EqualTo(array[25]));
        }

        [Test]
        public void BuildArrayElementWriter_ItWorks()
        {
            Action<Array, int, object> arrayElementWriter = ArrayAccessorBuilder.BuildArrayElementWriter(typeof (double));

            arrayElementWriter(array, 25, number);

            Assert.That(array[25], Is.EqualTo(number));
        }
    }
}