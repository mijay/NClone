using System;
using JetBrains.Annotations;

namespace mijay.Utils
{
    /// <summary>
    /// Collection of guard methods that aim to protect against invalid arguments/states.
    /// </summary>
    public static class Guard
    {
        /// <summary>
        /// Verifies that <paramref name="condition"/> is <c>true</c>.
        /// Otherwise throw <see cref="ArgumentException"/> with <see cref="Exception.Message"/> build from <paramref name="errorDescriptionFormat"/>.
        /// </summary>
        [StringFormatMethod("errorDescriptionFormat"), ContractAnnotation("condition: false => halt")]
        public static void AgainstViolation(bool condition, string errorDescriptionFormat, params object[] args)
        {
            if (!condition)
                throw new ArgumentException(string.Format(errorDescriptionFormat, args));
        }

        /// <summary>
        /// Verifies that <paramref name="argument"/> named as <paramref name="argumentName"/> is not <c>null</c>.
        /// Throws <see cref="NullReferenceException"/> otherwise.
        /// </summary>
        [ContractAnnotation("argument: null => halt")]
        public static void AgainstNull(object argument, [InvokerParameterName] string argumentName)
        {
            if (argument == null)
                throw new ArgumentNullException(argumentName);
        }
    }
}