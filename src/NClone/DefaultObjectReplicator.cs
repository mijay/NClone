using NClone.MetadataProviders;
using NClone.ObjectReplication;

namespace NClone
{
    public static class DefaultObjectReplicator
    {
        private static readonly IObjectReplicator instance = new ObjectReplicator(new ConventionalMetadataProvider());

        public static T Replicate<T>(T source)
        {
            return instance.Replicate(source);
        }
    }
}