using System;
using System.Threading;
using Akka.Actor;
using Akka.Configuration;
using BenchmarkDotNet.Attributes;

namespace Akka.Benchmarks.Actor
{
    [Config(typeof(MyConfig))]
    [MemoryDiagnoser]
    [BenchmarkCategory("actor")]
    public class ActorSystemBenchmarks
    {
        private readonly Config config = ConfigurationFactory.ParseString("akka.suppress-json-serializer-warning=true");

        [Benchmark]
        public ActorSystem ActorSystem_Create()
        {
            return ActorSystem.Create("test", config);
        }
    }
}
