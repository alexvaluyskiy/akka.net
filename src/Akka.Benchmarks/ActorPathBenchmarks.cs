using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
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
            Add(Job.Default.With(Runtime.Clr).With(Jit.RyuJit).With(Platform.X64).WithId("NET4.7_RyuJIT-x64"));
            Add(Job.Default.With(Runtime.Clr).With(Jit.LegacyJit).With(Platform.X86).WithId("NET4.7_LegacyJit-x86"));
            Add(Job.Default.With(Runtime.Clr).With(Jit.LegacyJit).With(Platform.X64).WithId("NET4.7_LegacyJit-x64"));
            Add(Job.Default.With(Runtime.Core).With(CsProjCoreToolchain.NetCoreApp11).WithId("Core1.1-x64"));
        }
    }

    [Config(typeof(MyConfig))]
    [MemoryDiagnoser]
    public class ActorPathBenchmarks
    {
        private static readonly ActorPath TestActorPath = ActorPath.Parse("akka.tcp://Sys@localhost:9091/user/foo");
        private static readonly RootActorPath RootAddress = new RootActorPath(Address.AllSystems);

        [Benchmark]
        public ActorPath ActorPath_Parse_local_address()
        {
            return ActorPath.Parse("akka://Sys/user/foo");
        }

        [Benchmark]
        public ActorPath ActorPath_Parse_remote_address()
        {
            return ActorPath.Parse("akka.tcp://Sys@localhost:9091/user/foo");
        }

        [Benchmark]
        public ActorPath ActorPath_TryParse_local_address()
        {
            ActorPath target;
            ActorPath.TryParse("akka://Sys/user/foo", out target);
            return target;
        }

        [Benchmark]
        public ActorPath ActorPath_TryParse_remote_address()
        {
            ActorPath target;
            ActorPath.TryParse("akka.tcp://Sys@localhost:9091/user/foo", out target);
            return target;
        }

        [Benchmark]
        public Address ActorPath_TryParseAddress_local_address()
        {
            Address target;
            ActorPath.TryParseAddress("akka://Sys/user/foo", out target);
            return target;
        }

        [Benchmark]
        public Address ActorPath_TryParseAddress_remote_address()
        {
            Address target;
            ActorPath.TryParseAddress("akka.tcp://Sys@localhost:9091/user/foo", out target);
            return target;
        }

        [Benchmark]
        public IReadOnlyList<string> ActorPath_Elements()
        {
            return TestActorPath.Elements;
        }

        [Benchmark]
        public ActorPath ActorPath_Concat_Operator()
        {
            return RootAddress / "user" / "foo";
        }
    }
}
