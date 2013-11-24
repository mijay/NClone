using System.Diagnostics;

namespace mijay.Utils
{
    /// <summary>
    /// Collection of extensions methods for objects of any type.
    /// </summary>
    [DebuggerStepThrough]
    public static class ObjectExtensions
    {
        /// <summary>
        /// Casts given <paramref name="obj"/> to type <typeparamref name="T"/>.
        /// </summary>
        public static T As<T>(this object @obj)
        {
            return (T) @obj;
        }
    }
}