using System;

namespace NClone.Profile
{
    internal static class Program
    {
        public static void Main()
        {
            //SomeClass source = CreateData();

            //source = Clone.ObjectGraph(source);

            //Console.WriteLine(source.Property.Sum(x => x.field.Length));

            var source = new SomeClass4
                         {
                             field = 42,
                             Property = new SomeClass3 { Property = new SomeClass2 { field = "string" } }
                         };

            for (int i = 0; i < 2000000; ++i) {
                source = Clone.ObjectGraph(source);
            }

            Console.WriteLine(source.Property.Property.field.Length);
        }

        private static SomeClass CreateData()
        {
            var source = new SomeClass { Property = new SomeClass2[2000000] };
            for (int i = 0; i < source.Property.Length; i++)
                source.Property[i] = new SomeClass2 { field = i.ToString() };
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

        private class SomeClass3
        {
            public SomeClass2 Property { get; set; }
        }

        private class SomeClass4
        {
            public int field;
            public SomeClass3 Property { get; set; }
        }
    }
}