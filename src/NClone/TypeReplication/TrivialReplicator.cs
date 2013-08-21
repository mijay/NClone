namespace NClone.TypeReplication
{
    internal class TrivialReplicator<TType>: ITypeReplicator<TType>
    {
        public TType Replicate(TType source)
        {
            return source;
        }

        public bool IsRedundant
        {
            get { return true; }
        }
    }
}