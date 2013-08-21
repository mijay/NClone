using System.Reflection;

namespace NClone.MemberCopying
{
    /// <summary>
    /// Object which is able to copy value of <see cref="Field"/> between containers of type <typeparamref name="TContainer"/>.
    /// Value is optionally cloned (deep copied) during copying.
    /// </summary>
    internal interface IMemberCopier<TContainer>
    {
        /// <summary>
        /// Field, which value can be copied by this <see cref="IMemberCopier{TContainer}"/>.
        /// </summary>
        FieldInfo Field { get; }

        /// <summary>
        /// Copies a value of <see cref="Field"/> from <paramref name="source"/> to <paramref name="destination"/> and return modified object.
        /// Value of a field is cloned (deep copied) during copying if <see cref="IsTrivial"/> is <c>false</c>.
        /// </summary>
        TContainer Copy(TContainer source, TContainer destination);

        /// <summary>
        /// Indicates whether this <see cref="IMemberCopier{TContainer}"/> is trivial, i.e. doesn't clone copied value.
        /// </summary>
        bool IsTrivial { get; }
    }
}