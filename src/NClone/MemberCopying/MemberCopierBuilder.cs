using System;
using System.Reflection;

namespace NClone.MemberCopying
{
    internal class MemberCopierBuilder: IMemberCopierBuilder
    {
        public IMemberCopier<TContainer> BuildFor<TContainer>(FieldInfo field)
        {
            throw new NotImplementedException();
        }
    }
}