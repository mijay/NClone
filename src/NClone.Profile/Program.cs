using System;
using System.Linq;

namespace NClone.Profile
{
    internal static class Program
    {
        public static void Main()
        {
            SomeClass source = CreateData();

            source = Clone.ObjectGraph(source);

            Console.WriteLine(source.Property.Sum(x => x.field.Length));
        }

        private static SomeClass CreateData()
        {
            var source = new SomeClass
            {
                Property = Enumerable.Range(0, 200000)
                    .Select(i => new SomeClass2 { field = i.ToString() })
                    .ToArray()
            };
            return source;
        }

        private class SomeClass
        {
            public SomeClass2[] Property { get; set; }
        }

        private class SomeClass2
        {
            public string field;
        }
    }
}