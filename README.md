NClone - is a generic implementation of deep copy for arbitrary .NET objects. Inspired by lazy peoples, who are tired of implementing ICloneable manually.

### Features

NClone is:

1. Easy to use. NClone just does his work, and does not bother developers with boilerplate code. The only thing you need to write is:
    ```csharp
    var clone = DefaultObjectReplicator.Replicate(obj);
    ```

1. Fast. NClone uses lightweight code generation in CIL instead of reflection, which gives significant (5-20x) speedup. Employed smart caching guarantees that every work will be done only if needed and at most once.

1. Customizable. You can tell NClone, which types or which members of types should not be deep-copied, or should even be nulled for freshly created clones. The easiest way to do so, is to apply `CustomReplicationBehavior` attribute, but for curious guys we have deeper holes

1. Smart. NClone knows that (in most of the cases): structures are immutable; event subscribers are stick to concrete object instance; cloning lazy objects is evil; etc…

### How to use NClone

In most of the cases the only thing you need to do, is:

1. Reference NClone.
1. Apply `CustomReplicationBehaviorAttribute` to members/types that does not follow conventions (if any).
1. Use static method `DefaultObjectReplicator.Replicate<T>(T source)` for cloning.

If you need more extensibility, consider using class `ObjectReplicator` with your custom implementation of `IMetadataProvider`.
