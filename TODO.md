### *Must* todos

1. Move mijay.Utils to separate GitHub repository
2. Replcade Guard static class by CodeContracts

### *Can* todos

1. Hooks
2. Introduce fluent API for configuration in addition to attributes
3. Make cloning muti-threaded
4. Support multi-dimentional arrays *easy*

### Benchmark results
```
GraphWithBlobReplicationCompetition/ReplicatorClone    : 492ms 1103681 ticks [Error = 63.30%, StdDev = 97.06]
GraphWithBlobReplicationCompetition/ReflectionClone    : 578ms 1296721 ticks [Error = 35.52%, StdDev = 54.65]
SimpleGraphReplicationCompetition/ManyRuns             : 542ms 1215486 ticks [Error = 02.78%, StdDev =  4.22]
SimpleGraphReplicationCompetition/ReflectionClone      : 847ms 1899013 ticks [Error = 00.83%, StdDev =  2.26]
```
