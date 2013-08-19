namespace Core
{
    public static class ObjectExtensions
    {
        public static T CastTo<T>(this object value)
        {
            return (T) value;
        }
    }
}