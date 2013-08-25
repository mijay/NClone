using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace NClone.Shared
{
    public static class Guard
    {
        [Conditional("DEBUG"), StringFormatMethod("errorDescriptionFormat")]
        public static void AgainstViolation(bool condition, string errorDescriptionFormat, params object[] args)
        {
            if (!condition)
                throw new ArgumentException(string.Format(errorDescriptionFormat, args));
        }

        [Conditional("DEBUG"), StringFormatMethod("errorDescriptionFormat")]
        public static void AgainstViolation<TException>(bool condition, string errorDescriptionFormat = null, params object[] args)
        {
            if (!condition)
                throw (Exception) Activator.CreateInstance(typeof (TException), errorDescriptionFormat != null
                    ? new object[] { string.Format(errorDescriptionFormat, args) }
                    : new object[0]);
        }

        [Conditional("DEBUG")]
        public static void AgainstNull(object argument, [InvokerParameterName] string argumentName)
        {
            if (argument == null)
                throw new ArgumentNullException(argumentName);
        }
    }
}