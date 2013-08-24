using System;
using NUnit.Framework;

namespace NClone.Tests
{
    public class TestBase
    {
        private readonly Random random = new Random();

        [SetUp]
        protected virtual void SetUp()
        {
        }

        protected int RandomInt()
        {
            return random.Next();
        }
    }
}