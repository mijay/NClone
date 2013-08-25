namespace NClone.MemberAccess
{
    public interface IMemberAccessor<TEntity, TMember>
    {
        TEntity SetMember(TEntity entity, TMember memberValue);
        TMember GetMember(TEntity entity);
        bool CanGet { get; }
        bool CanSet { get; }
    }
}