using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace NClone.Shared
{
    /// <summary>
    /// Collection of guard methods that aim to protect against invalid arguments/states.
    /// Executed only if project is compiled in debug mode.
    /// </summary>
    public static class Guard
    {
        /// <summary>
        /// Verifies that <paramref name="condition"/> is <c>true</c>.
        /// Otherwise throw <see cref="ArgumentException"/> with <see cref="Exception.Message"/> build from <paramref name="errorDescriptionFormat"/>.
        /// </summary>
        [Conditional("DEBUG"), StringFormatMethod("errorDescriptionFormat")]
        public static void AgainstViolation(bool condition, string errorDescriptionFormat, params object[] args)
        {
            AgainstViolation<ArgumentException>(condition, errorDescriptionFormat, args);
        }

        /// <summary>
        /// Verifies that <paramref name="condition"/> is <c>true</c>.
        /// Otherwise throw <typeparamref name="TException"/> with <see cref="Exception.Message"/> build from <paramref name="errorDescriptionFormat"/>.
        /// </summary>
        /// <remarks>
        /// If no <paramref name="errorDescriptionFormat"/> is provided, then resulting <typeparamref name="TException"/> is build using parameterless
        /// constructor. Otherwise constructor with single <c>string</c> parameter is used.
        /// </remarks>
        [Conditional("DEBUG"), StringFormatMethod("errorDescriptionFormat")]
        public static void AgainstViolation<TException>(bool condition, string errorDescriptionFormat = null, params object[] args)
        {
            if (!condition)
                throw (Exception) Activator.CreateInstance(typeof (TException),
                    errorDescriptionFormat != null
                        ? new object[] { string.Format(errorDescriptionFormat, args) }
                        : new object[0]);
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