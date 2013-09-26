using System.Diagnostics;

namespace NClone.Shared
{
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