namespace NClone.ReplicationStrategies
{
    /// <summary>
    /// Dummy implementation of <see cref="IReplicationStrategy"/>, which <see cref="Replicate"/> method always return <c>null</c>.
    /// </summary>
    internal class IgnoringReplicationStrategy: IReplicationStrategy
    {
        /// <summary>
        /// The only instance of <see cref="IgnoringReplicationStrategy"/>.
        /// </summary>
        public static readonly IReplicationStrategy Instance = new IgnoringReplicationStrategy();

        private IgnoringReplicationStrategy() { }

        public object Replicate(object source)
        {
            return null;
        }
    }
}