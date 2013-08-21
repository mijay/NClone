using System.Reflection;

namespace NClone.MemberCopying
{
	internal class MemberCopier<TContainer>: IMemberCopier<TContainer>
	{
	    public MemberCopier(FieldInfo field, bool isTrivial)
	    {
	        Field = field;
	        IsTrivial = isTrivial;
	    }

	    public FieldInfo Field { get; private set; }

	    public TContainer Copy(TContainer source, TContainer destination)
	    {
	        throw new System.NotImplementedException();
	    }

	    public bool IsTrivial { get; private set; }
	}
}