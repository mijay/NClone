namespace NClone.SpecificTypeReplicators
{
    /// <summary>
    /// Dummy implementation of <see cref="IReplicationStrategy"/>, which <see cref="Replicate"/> method just returns given argument.
    /// </summary>
    internal class CopyOnlyReplicationStategy: IReplicationStrategy
    {
        /// <summary>
        /// The only instance of <see cref="CopyOnlyReplicationStategy"/>.
        /// </summary>
        public static readonly IReplicationStrategy Instance = new CopyOnlyReplicationStategy();

        private CopyOnlyReplicationStategy() { }

        public object Replicate(object source)
        {
            return source;
        }
    }
}