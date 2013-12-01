using System;
using System.Linq;

namespace NClone.Profile
{
    internal static class Program
    {
        public static void Main()
        {
            var source = new SomeClass
                         {
                             field = 42,
                             Property = new SomeClass2 { Property = new SomeClass3 { field = new int[1240] } }
                         };

            for (int i = 0; i < 200000; ++i)
                source = Clone.ObjectGraph(source);

            Console.WriteLine(source.field);
            Console.WriteLine(source.Property.Property.field.Sum());
        }

        private class SomeClass
        {
            public int field;
            public SomeClass2 Property { get; set; }
        }

        private class SomeClass2
        {
            public SomeClass3 Property { get; set; }
        }

        private class SomeClass3
        {
            public int[] field;
        }
    }
}