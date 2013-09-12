namespace NClone.SpecificTypeReplicators
{
    /// <summary>
    /// Dummy implementation of <see cref="IReplicationStrategy"/>, which <see cref="Replicate"/> method just returns given argument.
    /// </summary>
    internal class NonReplicatingStrategy: IReplicationStrategy
    {
        /// <summary>
        /// The only instance of <see cref="NonReplicatingStrategy"/>.
        /// </summary>
        public static readonly IReplicationStrategy Instance = new NonReplicatingStrategy();

        private NonReplicatingStrategy() { }

        public object Replicate(object source)
        {
            return source;
        }
    }
}