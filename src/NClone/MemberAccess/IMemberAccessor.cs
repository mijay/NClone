using System;

namespace NClone.MemberAccess
{
    /// <summary>
    /// Represents access to a specific member of a specific type.
    /// </summary>
    public interface IMemberAccessor
    {
        /// <summary>
        /// Indicates whether current <see cref="IMemberAccessor"/> can <see cref="GetMember"/>.
        /// </summary>
        bool CanGet { get; }

        /// <summary>
        /// Indicates whether current <see cref="IMemberAccessor"/> can <see cref="SetMember"/>.
        /// </summary>
        bool CanSet { get; }

        /// <summary>
        /// Sets value of accessed member in <paramref name="container"/> to <paramref name="memberValue"/>
        /// and returns modified <paramref name="container"/>.
        /// </summary>
        /// <remarks>
        /// <para>This method returns value, because <paramref name="container"/> can be by-value type.
        /// Such types cannot be edited in place.</para>
        /// 
        /// <para>If <paramref name="container"/> is by-reference type, then returned value will be
        /// equivalent to the <paramref name="container"/>.</para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">When <see cref="CanSet"/> is <c>false</c>.</exception>
        object SetMember(object container, object memberValue);

        /// <summary>
        /// Gets value of accessed member in <paramref name="container"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">When <see cref="CanGet"/> is <c>false</c>.</exception>
        object GetMember(object container);
    }
}