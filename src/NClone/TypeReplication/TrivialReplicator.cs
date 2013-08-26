using System;

namespace NClone.TypeReplication
{
    /// <summary>
    /// Trivial implementation of <see cref="IEntityReplicator"/>, which always omit copying.
    /// </summary>
    internal class TrivialReplicator: IEntityReplicator
    {
        public TrivialReplicator(Type entityType)
        {
            EntityType = entityType;
        }

        public Type EntityType { get; private set; }

        public object Replicate(object source)
        {
            return source;
        }

        public bool IsTrivial
        {
            get { return true; }
        }
    }
}