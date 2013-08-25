using System;

namespace NClone.Tests
{
    public static class Repeat
    {
        public static T[] Times<T>(int times, Func<T> builder)
        {
            var result = new T[times];
            for (var i = 0; i < times; i++)
                result[i] = builder();
            return result;
        }

        public static T[] Twice<T>(Func<T> builder)
        {
            return Times(2, builder);
        }
    }
}