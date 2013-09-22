using NClone.MetadataProviders;
using NClone.ObjectReplicators;
using NClone.Shared;

namespace NClone
{
    public static class DefaultObjectReplicator
    {
        private static readonly IObjectReplicator instance = new ObjectReplicator(new ConventionalMetadataProvider());

        public static T Replicate<T>(T source)
        {
            return instance.Replicate(source).As<T>();
        }
    }
}