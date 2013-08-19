namespace Core.MemberAccess
{
    public interface IMemberAccessor
    {
        object GetValue(object container);
        void SetValue(object value, object container);
    }
}