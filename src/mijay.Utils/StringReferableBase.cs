using JetBrains.Annotations;

namespace mijay.Utils
{
    /// <summary>
    /// Class which instance can be uniquely identified by some string. 
    /// </summary>
    public abstract class StringReferableBase
    {
        private readonly string key;

        [StringFormatMethod("keyFormat")]
        protected StringReferableBase(string keyFormat, params object[] keyArguments): this(string.Format(keyFormat, keyArguments))
        {
        }

        protected StringReferableBase(string key)
        {
            Guard.AgainstNull(key, "key");
            this.key = key;
        }

        public override int GetHashCode()
        {
            return key.GetHashCode();
        }

        public override string ToString()
        {
            return key;
        }

        public override bool Equals(object obj)
        {
            return obj is StringReferableBase && ((StringReferableBase) obj).key == key;
        }
    }
}