using NClone.Annotation;
using NClone.ObjectReplicators;
using NClone.Shared;

namespace NClone
{
    public static class DefaultObjectReplicator
    {
        private static readonly IObjectReplicator instance = new ObjectReplicator(new DefaultMetadataProvider());

        public static T Replicate<T>(T source)
        {
            return instance.Replicate(source).As<T>();
        }
    }
}