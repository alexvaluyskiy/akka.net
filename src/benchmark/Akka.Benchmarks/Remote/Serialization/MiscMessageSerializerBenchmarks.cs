using System;
using System.Collections.Generic;
using Akka.Actor;
using BenchmarkDotNet.Attributes;
using Akka.Remote.Serialization;
using Akka.Configuration;
using Akka.Dispatch;
using Akka.Remote.Configuration;
using Akka.Remote.Routing;
using Akka.Routing;

namespace Akka.Benchmarks
{
    [Config(typeof(MyConfig))]
    [MemoryDiagnoser]
    [BenchmarkCategory("serialization")]
    public class MiscMessageSerializerBenchmarks
    {
        private readonly ActorSystem System;
        private readonly MiscMessageSerializer MiscMessageSerializer;

        // Test objects
        IActorRef LocalActorRef { get; }
        Identify TestIdentify { get; } = new Identify("test");
        ActorIdentity TestActorIdentity { get; } = new ActorIdentity("test", null);
        Config TestConfig { get; }
        FromConfig TestFromConfig { get; } = new FromConfig(new DefaultResizer(1, 2, 1, 0.2D, 1, 2D, 5), SupervisorStrategy.DefaultStrategy, Dispatchers.DefaultDispatcherId);
        RoundRobinPool TestRoundRobinPool { get; } = new RoundRobinPool(5, new DefaultResizer(1, 2, 1, 0.2D, 1, 2D, 5), SupervisorStrategy.DefaultStrategy, Dispatchers.DefaultDispatcherId);
        BroadcastPool TestBroadcastPool { get; } = new BroadcastPool(5, new DefaultResizer(1, 2, 1, 0.2D, 1, 2D, 5), SupervisorStrategy.DefaultStrategy, Dispatchers.DefaultDispatcherId);
        RandomPool TestRandomPool { get; } = new RandomPool(5, new DefaultResizer(1, 2, 1, 0.2D, 1, 2D, 5), SupervisorStrategy.DefaultStrategy, Dispatchers.DefaultDispatcherId);
        ScatterGatherFirstCompletedPool TestScatterGatherFirstCompletedPool { get; } = new ScatterGatherFirstCompletedPool(5, new DefaultResizer(1, 2, 1, 0.2D, 1, 2D, 5), TimeSpan.FromMinutes(5), SupervisorStrategy.DefaultStrategy, Dispatchers.DefaultDispatcherId);
        TailChoppingPool TestTailChoppingPool { get; } = new TailChoppingPool(5, new DefaultResizer(1, 2, 1, 0.2D, 1, 2D, 5), SupervisorStrategy.DefaultStrategy, Dispatchers.DefaultDispatcherId, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(6));
        RemoteRouterConfig RemoteRouterConfig { get; }

        public MiscMessageSerializerBenchmarks()
        {
            var config = ConfigurationFactory.ParseString("akka.suppress-json-serializer-warning=true").WithFallback(RemoteConfigFactory.Default());
            System = ActorSystem.Create("RemoteSerializationBenchmarks", config);
            MiscMessageSerializer = new MiscMessageSerializer(System as ExtendedActorSystem);
            LocalActorRef = System.ActorOf(Props.Empty);
            TestConfig = ConfigurationFactory.ParseString(@"
                akka.persistence.query.journal.sql {
                    class = ""Akka.Persistence.Query.Sql.SqlReadJournalProvider, Akka.Persistence.Query.Sql""
                    write-plugin = ""
                    refresh-interval = 3s
                    max-buffer-size = 100
                }");

            RemoteRouterConfig = new RemoteRouterConfig(TestRoundRobinPool, new List<Address>
            {
                new Address("akka.tcp", "Sys", "test", 5)
            });
        }

        [Benchmark]
        public object MiscMessageSerializer_Serialize_Identify()
        {
            var bytes = MiscMessageSerializer.ToBinary(TestIdentify);
            var manifest = MiscMessageSerializer.Manifest(TestIdentify);
            return MiscMessageSerializer.FromBinary(bytes, manifest);
        }

        [Benchmark]
        public object MiscMessageSerializer_Serialize_ActorIdentity()
        {
            var bytes = MiscMessageSerializer.ToBinary(TestActorIdentity);
            var manifest = MiscMessageSerializer.Manifest(TestActorIdentity);
            return MiscMessageSerializer.FromBinary(bytes, manifest);
        }

        [Benchmark]
        public object MiscMessageSerializer_Serialize_ActorRef()
        {
            var bytes = MiscMessageSerializer.ToBinary(LocalActorRef);
            var manifest = MiscMessageSerializer.Manifest(LocalActorRef);
            return MiscMessageSerializer.FromBinary(bytes, manifest);
        }

        [Benchmark]
        public object MiscMessageSerializer_Serialize_Config()
        {
            var bytes = MiscMessageSerializer.ToBinary(TestConfig);
            var manifest = MiscMessageSerializer.Manifest(TestConfig);
            return MiscMessageSerializer.FromBinary(bytes, manifest);
        }

        [Benchmark]
        public object MiscMessageSerializer_Serialize_FromConfig()
        {
            var bytes = MiscMessageSerializer.ToBinary(TestFromConfig);
            var manifest = MiscMessageSerializer.Manifest(TestFromConfig);
            return MiscMessageSerializer.FromBinary(bytes, manifest);
        }

        [Benchmark]
        public object MiscMessageSerializer_Serialize_RoundRobinPool()
        {
            var bytes = MiscMessageSerializer.ToBinary(TestRoundRobinPool);
            var manifest = MiscMessageSerializer.Manifest(TestRoundRobinPool);
            return MiscMessageSerializer.FromBinary(bytes, manifest);
        }

        [Benchmark]
        public object MiscMessageSerializer_Serialize_BroadcastPool()
        {
            var bytes = MiscMessageSerializer.ToBinary(TestBroadcastPool);
            var manifest = MiscMessageSerializer.Manifest(TestBroadcastPool);
            return MiscMessageSerializer.FromBinary(bytes, manifest);
        }

        [Benchmark]
        public object MiscMessageSerializer_Serialize_RandomPool()
        {
            var bytes = MiscMessageSerializer.ToBinary(TestRandomPool);
            var manifest = MiscMessageSerializer.Manifest(TestRandomPool);
            return MiscMessageSerializer.FromBinary(bytes, manifest);
        }

        [Benchmark]
        public object MiscMessageSerializer_Serialize_ScatterGatherFirstCompletedPool()
        {
            var bytes = MiscMessageSerializer.ToBinary(TestScatterGatherFirstCompletedPool);
            var manifest = MiscMessageSerializer.Manifest(TestScatterGatherFirstCompletedPool);
            return MiscMessageSerializer.FromBinary(bytes, manifest);
        }

        [Benchmark]
        public object MiscMessageSerializer_Serialize_TailChoppingPool()
        {
            var bytes = MiscMessageSerializer.ToBinary(TestTailChoppingPool);
            var manifest = MiscMessageSerializer.Manifest(TestTailChoppingPool);
            return MiscMessageSerializer.FromBinary(bytes, manifest);
        }

        [Benchmark]
        public object MiscMessageSerializer_Serialize_RemoteRouterConfig()
        {
            var bytes = MiscMessageSerializer.ToBinary(RemoteRouterConfig);
            var manifest = MiscMessageSerializer.Manifest(RemoteRouterConfig);
            return MiscMessageSerializer.FromBinary(bytes, manifest);
        }
    }
}
