using System.Collections.Generic;
using System.Runtime;

namespace mijay.Utils.Comparers
{
    public class ReferenceEqualityComparer: IEqualityComparer<object>
    {
        public static readonly IEqualityComparer<object> Instance = new ReferenceEqualityComparer();

        private ReferenceEqualityComparer()
        {
        }

        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public new bool Equals(object x, object y)
        {
            return ReferenceEquals(x, y);
        }

        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public int GetHashCode(object obj)
        {
            return ReferenceEquals(obj, null) ? 0 : obj.GetHashCode();
        }
    }
}