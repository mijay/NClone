using System;
using System.Linq;
using NClone.MemberAccess;
using NUnit.Framework;

namespace NClone.Tests.MemberAccess
{
    public class ArrayAccessorBuilderTest: TestBase
    {
        private double[] array;
        private IArrayAccessor arrayAccessor;
        private double number;

        protected override void SetUp()
        {
            base.SetUp();
            var random = new Random();

            array = Enumerable.Range(0, 100)
                .Select(_ => random.NextDouble())
                .ToArray();
            number = random.NextDouble();
            arrayAccessor = ArrayAccessorBuilder.BuildForArrayOf(typeof (double));
        }

        [Test]
        public void BuildArrayElementReader_ItWorks()
        {
            object result = arrayAccessor.GetElement(array, 25);

            Assert.That(result, Is.EqualTo(array[25]));
        }

        [Test]
        public void BuildArrayElementWriter_ItWorks()
        {
            arrayAccessor.SetElement(array, 25, number);

            Assert.That(array[25], Is.EqualTo(number));
        }
    }
}