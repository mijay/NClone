using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Core.MemberAccess;

namespace Core.Clone
{
    public static class ObjectReplicator
    {
        private static readonly ConcurrentDictionary<Type, IMemberAccessor[]> typeFields =
            new ConcurrentDictionary<Type, IMemberAccessor[]>();

        public static T Clone<T>(T value)
        {
            return (T) Clone((object) value);
        }

        private static object Clone(object value)
        {
            if (value == null || value is string)
                return value;
            var type = value.GetType();
            if (type.IsPrimitive)
                return value;

            var result = FormatterServices.GetUninitializedObject(type);
            foreach (var memberAccessor in typeFields.GetOrAdd(type, BuildMemberAccessors))
                memberAccessor.SetValue(Clone(memberAccessor.GetValue(value)), result);
            return result;
        }

        private static IMemberAccessor[] BuildMemberAccessors(Type t)
        {
            return t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Select(MemberAccessorFactory.BuildAccessor)
                    .ToArray();
        }
    }
}