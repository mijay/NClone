namespace NClone.SpecificTypeReplicators
{
    /// <summary>
    /// Dummy implementation of <see cref="ISpecificTypeReplicator"/>, which <see cref="Replicate"/> method just returns given argument.
    /// </summary>
    internal class DummyReplicator: ISpecificTypeReplicator
    {
        /// <summary>
        /// The only instance of <see cref="DummyReplicator"/>.
        /// </summary>
        public static readonly ISpecificTypeReplicator Instance = new DummyReplicator();

        private DummyReplicator() { }

        public object Replicate(object source)
        {
            return source;
        }
    }
}