namespace NClone.TypeReplication
{
    internal interface ITypeReplicator<TType>
    {
        TType Replicate(TType source);
        bool IsRedundant { get; }
    }
}