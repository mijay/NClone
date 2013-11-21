using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace NClone.Utils
{
    /// <summary>
    /// Collection of guard methods that aim to protect against invalid arguments/states.
    /// Executed only if project is compiled in debug mode.
    /// </summary>
    internal static class Guard
    {
        /// <summary>
        /// Verifies that <paramref name="condition"/> is <c>true</c>.
        /// Otherwise throw <see cref="ArgumentException"/> with <see cref="Exception.Message"/> build from <paramref name="errorDescriptionFormat"/>.
        /// </summary>
        [Conditional("DEBUG"), StringFormatMethod("errorDescriptionFormat")]
        public static void AgainstViolation(bool condition, string errorDescriptionFormat, params object[] args)
        {
            if (!condition)
                throw new ArgumentException(string.Format(errorDescriptionFormat, args));
        }

        /// <summary>
        /// Verifies that <paramref name="argument"/> named as <paramref name="argumentName"/> is not <c>null</c>.
        /// Throws <see cref="NullReferenceException"/> otherwise.
        /// </summary>
        [Conditional("DEBUG")]
        public static void AgainstNull(object argument, [InvokerParameterName] string argumentName)
        {
            if (argument == null)
                throw new ArgumentNullException(argumentName);
        }
    }
}