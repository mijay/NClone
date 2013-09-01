namespace NClone.EntityReplicators
{
    /// <summary>
    /// Dummy implementation of <see cref="IEntityReplicator"/>, which <see cref="Replicate"/> method just returns given argument.
    /// </summary>
    internal class DummyReplicator: IEntityReplicator
    {
        /// <summary>
        /// The only instance of <see cref="DummyReplicator"/>.
        /// </summary>
        public static readonly IEntityReplicator Instance = new DummyReplicator();

        private DummyReplicator() { }

        public object Replicate(object source)
        {
            return source;
        }
    }
}