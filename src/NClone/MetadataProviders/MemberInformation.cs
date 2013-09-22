using System.Reflection;

namespace NClone.MetadataProviders
{
    /// <summary>
    /// Value-type for describing declaration-side behavior for members.
    /// </summary>
    public class MemberInformation
    {
        public FieldInfo Member { get; private set; }
        public ReplicationBehavior Behavior { get; private set; }

        public MemberInformation(FieldInfo member, ReplicationBehavior behavior)
        {
            Member = member;
            Behavior = behavior;
        }
    }
}