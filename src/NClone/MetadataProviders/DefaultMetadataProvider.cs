using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using mijay.Utils;
using mijay.Utils.Reflection;

namespace NClone.MetadataProviders
{
    /// <summary>
    /// Implementation of <see cref="IMetadataProvider"/> that provides basic and always applicable functionality.
    /// </summary>
    /// <remarks>
    /// <para><see cref="DefaultMetadataProvider"/> defines that:
    /// <list type="number">
    /// <item>Primitive types, enums and <c>string</c> use <see cref="ReplicationBehavior.Copy"/>.</item>
    /// <item><see cref="Nullable{T}"/> types inherit <see cref="ReplicationBehavior"/> from the underlying type.</item>
    /// <item>For each <see cref="ReplicationBehavior.DeepCopy"/>-ied type all fields should be deep copied.</item>
    /// </list>
    /// </para>
    /// </remarks>
    public class DefaultMetadataProvider: IMetadataProvider
    {
        /// <inheritdoc />
        public ReplicationBehavior GetPerTypeBehavior(Type type)
        {
            Guard.AgainstNull(type, "type");
            return TryGetPerTypeBehavior(type) ?? ReplicationBehavior.DeepCopy;
        }

        /// <inheritdoc />
        public IEnumerable<FieldReplicationInfo> GetFieldsReplicationInfo(Type type)
        {
            Guard.AgainstNull(type, "type");

            return type.GetHierarchy()
                .SelectMany(GetFieldsDefinedIn)
                .Select(x => new FieldReplicationInfo(x.BackingField,
                    TryGetPerMemberReplicationBehavior(x) ?? ReplicationBehavior.DeepCopy));
        }

        /// <summary>
        /// Extension point. Try to find per-type <see cref="ReplicationBehavior"/> or
        /// return <c>null</c>, if current <see cref="IMetadataProvider"/> does not specify it.
        /// </summary>
        protected virtual ReplicationBehavior? TryGetPerTypeBehavior(Type entityType)
        {
            if (entityType.IsPrimitive || entityType.IsEnum || entityType == typeof (string))
                return ReplicationBehavior.Copy;
            return null;
        }

        /// <summary>
        /// Extension point. Try to find per-member <see cref="ReplicationBehavior"/> or
        /// return <c>null</c>, if current <see cref="IMetadataProvider"/> does not specify it.
        /// </summary>
        protected virtual ReplicationBehavior? TryGetPerMemberReplicationBehavior(CopyableFieldDescription fieldDescription)
        {
            return null;
        }

        private static IEnumerable<CopyableFieldDescription> GetFieldsDefinedIn(Type type)
        {
            MemberInfo[] typeMembers = type
                .GetMembers(BindingFlags.Public | BindingFlags.NonPublic
                            | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            return typeMembers
                .OfType<FieldInfo>()
                .Select(field => {
                            PropertyInfo declaringProperty;
                            if (field.TryGetDeclaringProperty(out declaringProperty))
                                return new CopyableFieldDescription(declaringProperty, field);

                            EventInfo declaringEvent;
                            if (field.TryGetDeclaringEvent(out declaringEvent))
                                return new CopyableFieldDescription(declaringEvent, field);

                            return new CopyableFieldDescription(field);
                        });
        }

        /// <summary>
        /// Description of one field in replicated type.
        /// </summary>
        /// <remarks>
        /// The main need of this class is to bind compiler-generated fields to 
        /// members that causes their generation.
        /// </remarks>
        // marked internal for testing purposes
        protected internal class CopyableFieldDescription
        {
            /// <summary>
            /// Create <see cref="CopyableFieldDescription"/> for common field.
            /// </summary>
            public CopyableFieldDescription(FieldInfo field)
            {
                BackingField = field;
                DeclaringMember = field;
            }

            /// <summary>
            /// Create <see cref="CopyableFieldDescription"/> for property with backing field.
            /// </summary>
            public CopyableFieldDescription(PropertyInfo autoProperty, FieldInfo backingField)
            {
                BackingField = backingField;
                DeclaringMember = autoProperty;
            }

            /// <summary>
            /// Create <see cref="CopyableFieldDescription"/> for event with backing field.
            /// </summary>
            public CopyableFieldDescription(EventInfo autoEvent, FieldInfo backingField)
            {
                BackingField = backingField;
                DeclaringMember = autoEvent;
            }

            /// <summary>
            /// <see cref="FieldInfo"/> of one field in replicated type.
            /// </summary>
            public FieldInfo BackingField { get; private set; }

            /// <summary>
            /// <see cref="MemberInfo"/> of member that causes emitting <see cref="BackingField"/>
            /// by compiler, e.g. event, auto-property, or field by itself.
            /// </summary>
            public MemberInfo DeclaringMember { get; private set; }
        }
    }
}