using System;
using JetBrains.Annotations;

namespace NClone.Shared
{
    public static class Guard
    {
        [StringFormatMethod("errorDescriptionFormat")]
        public static void Check(bool condition, string errorDescriptionFormat, params object[] args)
        {
            if (!condition)
                throw new ArgumentException(string.Format(errorDescriptionFormat, args));
        }
    }
}