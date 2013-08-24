using System;
using System.Reflection;

namespace NClone.MemberCopying
{
    /// <summary>
    /// Implementation of <see cref="IMemberCopierBuilder"/>.
    /// </summary>
    internal class MemberCopierBuilder: IMemberCopierBuilder
    {
        public IMemberCopier<TContainer> BuildFor<TContainer>(FieldInfo field)
        {
            throw new NotImplementedException();
        }
    }
}