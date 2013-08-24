namespace NClone.Tests.ExternalAssembly
{
    public class ClassWithInternalReadonlyField
    {
        internal readonly int field;

        public ClassWithInternalReadonlyField(int field)
        {
            this.field = field;
        }

        public int GetField()
        {
            return field;
        }
    }
}