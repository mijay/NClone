namespace NClone.TypeReplication
{
    /// <summary>
    /// Trivial implementation of <see cref="IEntityReplicator{TType}"/>, which always omit deep copying.
    /// </summary>
    internal class TrivialReplicator<TType>: IEntityReplicator<TType>
    {
        public TType Replicate(TType source)
        {
            return source;
        }

        public bool IsTrivial
        {
            get { return true; }
        }
    }
}