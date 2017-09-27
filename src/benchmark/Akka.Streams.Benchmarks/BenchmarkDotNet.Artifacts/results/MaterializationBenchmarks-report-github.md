``` ini

BenchmarkDotNet=v0.10.9, OS=Windows 10 Redstone 2 (10.0.15063)
Processor=Intel Core i7-7700K CPU 4.20GHz (Kaby Lake), ProcessorCount=8
.NET Core SDK=2.0.0
  [Host]      : .NET Core 2.0.0 (Framework 4.6.00001.0), 64bit RyuJIT
  NETCORE 2.0 : .NET Core 2.0.0 (Framework 4.6.00001.0), 64bit RyuJIT

Job=NETCORE 2.0  Platform=X64  Runtime=Core  
Server=True  Toolchain=CoreCsProj  

```
 |                                    Method |     Mean |    Error |   StdDev |  Gen 0 |  Gen 1 | Allocated |
 |------------------------------------------ |---------:|---------:|---------:|-------:|-------:|----------:|
 |                Single_materialization_run | 260.7 us | 4.871 us | 4.318 us | 0.7673 | 0.2325 |   29.2 KB |
 |            Single_materialization_runWith | 294.2 us | 5.652 us | 5.287 us | 0.7468 | 0.2011 |  29.42 KB |
 |              Source_Empty_materialization | 287.7 us | 6.893 us | 8.466 us | 0.9766 | 0.3357 |  32.67 KB |
 |         Source_Enumerator_materialization | 339.3 us | 6.738 us | 7.759 us | 1.3672 | 0.5469 |  41.46 KB |
 |         Source_Enumerable_materialization | 340.9 us | 8.143 us | 9.051 us | 1.0376 | 0.3357 |  41.44 KB |
 | Source_Single_with_Select_materialization | 322.6 us | 7.167 us | 8.253 us | 0.9988 | 0.3773 |  38.51 KB |
 |  Source_Single_with_Where_materialization | 324.2 us | 8.203 us | 8.423 us | 1.1985 | 0.5105 |  38.21 KB |
