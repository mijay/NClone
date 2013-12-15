using System.Collections.Generic;

namespace mijay.Utils.Comparers
{
    public class ReferenceEqualityComparer: IEqualityComparer<object>
    {
        public static readonly IEqualityComparer<object> Instance = new ReferenceEqualityComparer();

        private ReferenceEqualityComparer()
        {
        }

        public new bool Equals(object x, object y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(object obj)
        {
            return ReferenceEquals(obj, null) ? 0 : obj.GetHashCode();
        }
    }
}