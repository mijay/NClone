using System.Reflection;

namespace NClone.MemberCopying
{
    internal interface IMemberCopierBuilder
    {
        IMemberCopier<TContainer> BuildFor<TContainer>(FieldInfo field);
    }
}