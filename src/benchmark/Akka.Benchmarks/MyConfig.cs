using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.CsProj;

namespace Akka.Benchmarks
{
    public class MyConfig : ManualConfig
    {
        public MyConfig()
        {
            Add(Job.Default.With(Runtime.Clr).With(Jit.RyuJit).With(Platform.X64).WithId("NET4.7_RyuJIT-x64").WithGcServer(true));
            Add(Job.Default.With(Runtime.Core).With(CsProjCoreToolchain.NetCoreApp11).With(Platform.X64).WithId("NETCORE 2.0").WithGcServer(true));
        }
    }
}
