using NClone.MemberCopying;

namespace NClone.TypeReplication
{
    internal class TypeReplicatorBuilder
    {
        private readonly IMemberCopierBuilder memberCopierBuilder;

        public TypeReplicatorBuilder(IMemberCopierBuilder memberCopierBuilder)
        {
            this.memberCopierBuilder = memberCopierBuilder;
        }

        public IEntityReplicator<TType> BuildFor<TType>()
        {
            var type = typeof (TType);
            if (type == typeof (string) || type.IsEnum || type.IsPrimitive)
                return new TrivialReplicator<TType>();
            return type.IsValueType
                ? (IEntityReplicator<TType>) new StructureReplicator<TType>(memberCopierBuilder)
                : new ObjectReplicator<TType>(memberCopierBuilder);
            //todo: memorization
        }
    }
}