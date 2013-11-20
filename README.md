NClone - is the implementation of deep copy of arbitrary .NET objects. Inspired by lazy people who were tired of implementing ICloneable interface manually.

### Features

NClone is:

1. Easy to use. You need just one line of code to do all 'dirty job' of cloning — forget boilerplate `ICloneable.Clone` implementations like a bad dream:
    ```csharp
    var clone = DefaultObjectReplicator.Replicate(obj);
    ```

1. Fast. NClone uses lightweight code generation in CIL instead of reflection, which gives significant (5-20x) speedup. Employed smart caching guarantees that every work will be done at most once. So NClone can be used in production code.

1. Customizable. You can use `CustomReplicationBehavior` attribute to tell NClone, which types or type members should be skipped (or shallow copied) during cloning. For deeper customization NClone contain more extension points.

1. Smart. NClone knows that (in most of the cases): structures are immutable; event subscribers should not be cloned (and even copied) while cloning object instance; cloning lazy objects is bad-practice; etc…

### How to use NClone

In most of the cases the only thing you need to do is:

1. Reference NClone.
1. Use static method `DefaultObjectReplicator.Replicate<T>(T source)` for cloning.
1. If you need to change how NClone processes some types or type members during cloning, apply `CustomReplicationBehaviorAttribute` to them.


If you need more extensibility, consider using a new instance of `ObjectReplicator` class with your own custom implementation of `IMetadataProvider` interface.

### Known issues

NClone currently does not support cloning of cyclic object graphs.
