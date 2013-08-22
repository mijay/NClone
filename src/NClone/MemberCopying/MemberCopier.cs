using System;
using System.Reflection;

namespace NClone.MemberCopying
{
    /// <summary>
    /// Default implementation of <see cref="IMemberCopier{TContainer}"/>
    /// </summary>
    internal class MemberCopier<TContainer>: IMemberCopier<TContainer>
    {
        public MemberCopier(FieldInfo field, bool performsReplication)
        {
            Field = field;
            PerformsReplication = performsReplication;
        }

        public FieldInfo Field { get; private set; }

        public TContainer Copy(TContainer source, TContainer destination)
        {
            throw new NotImplementedException();
        }

        public bool PerformsReplication { get; private set; }
    }
}