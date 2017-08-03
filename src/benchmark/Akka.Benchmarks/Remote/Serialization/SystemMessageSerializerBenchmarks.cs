using System;
using System.Collections.Generic;
using Akka.Actor;
using BenchmarkDotNet.Attributes;
using Akka.Remote.Serialization;
using Akka.Configuration;
using Akka.Dispatch;
using Akka.Dispatch.SysMsg;
using Akka.Remote;
using Akka.Remote.Configuration;
using Akka.Remote.Routing;
using Akka.Routing;

namespace Akka.Benchmarks
{
    [Config(typeof(MyConfig))]
    [MemoryDiagnoser]
    [BenchmarkCategory("serialization")]
    public class SystemMessageSerializerBenchmarks
    {
        private readonly ActorSystem System;

        IActorRef LocalActorRef { get; }
        Create TestCreate { get; }
        Recreate TestRecreate { get; } = new Recreate(new ArgumentException());
        Supervise TestSupervise { get; }
        Watch TestWatch { get; }
        Unwatch TestUnwatch { get; }
        Failed TestFailed { get; }
        DeathWatchNotification TestDeathWatchNotification { get; }

        Identify TestIdentify { get; } = new Identify("test");
        ActorIdentity TestActorIdentity { get; } = new ActorIdentity("test", null);
        Config TestConfig { get; }
        FromConfig TestFromConfig { get; } = new FromConfig(new DefaultResizer(1, 2, 1, 0.2D, 1, 2D, 5), SupervisorStrategy.DefaultStrategy, Dispatchers.DefaultDispatcherId);
        RoundRobinPool TestRoundRobinPool { get; } = new RoundRobinPool(5, new DefaultResizer(1, 2, 1, 0.2D, 1, 2D, 5), SupervisorStrategy.DefaultStrategy, Dispatchers.DefaultDispatcherId);
        BroadcastPool TestBroadcastPool { get; } = new BroadcastPool(5, new DefaultResizer(1, 2, 1, 0.2D, 1, 2D, 5), SupervisorStrategy.DefaultStrategy, Dispatchers.DefaultDispatcherId);
        RandomPool TestRandomPool { get; } = new RandomPool(5, new DefaultResizer(1, 2, 1, 0.2D, 1, 2D, 5), SupervisorStrategy.DefaultStrategy, Dispatchers.DefaultDispatcherId);
        ScatterGatherFirstCompletedPool TestScatterGatherFirstCompletedPool { get; } = new ScatterGatherFirstCompletedPool(5, new DefaultResizer(1, 2, 1, 0.2D, 1, 2D, 5), TimeSpan.FromMinutes(5), SupervisorStrategy.DefaultStrategy, Dispatchers.DefaultDispatcherId);
        TailChoppingPool TestTailChoppingPool { get; } = new TailChoppingPool(5, new DefaultResizer(1, 2, 1, 0.2D, 1, 2D, 5), SupervisorStrategy.DefaultStrategy, Dispatchers.DefaultDispatcherId, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(6));
        ConsistentHashingPool TestConsistentHashingPool { get; } = new ConsistentHashingPool(5, new DefaultResizer(1, 2, 1, 0.2D, 1, 2D, 5), SupervisorStrategy.DefaultStrategy, Dispatchers.DefaultDispatcherId);
        RemoteRouterConfig RemoteRouterConfig { get; }

        public SystemMessageSerializerBenchmarks()
        {
            var config = ConfigurationFactory.ParseString("akka.suppress-json-serializer-warning=true").WithFallback(RemoteConfigFactory.Default());
            System = ActorSystem.Create("RemoteSerializationBenchmarks", config);
            LocalActorRef = System.ActorOf(Props.Empty);
            TestCreate = new Create(new ActorInitializationException(LocalActorRef, "message"));
            TestSupervise = new Supervise(LocalActorRef, true);
            TestWatch = new Watch((IInternalActorRef)LocalActorRef, (IInternalActorRef)LocalActorRef);
            TestUnwatch = new Unwatch((IInternalActorRef)LocalActorRef, (IInternalActorRef)LocalActorRef);
            TestFailed = new Failed(LocalActorRef, new ArgumentException(), 43534);
            TestDeathWatchNotification = new DeathWatchNotification(LocalActorRef, true, false);

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

        //[Benchmark]
        public object SystemMessageSerializer_Serialize_Create()
        {
            var payload = MessageSerializer.Serialize(System, null, TestCreate);
            return MessageSerializer.Deserialize(System, payload);
        }

        //[Benchmark]
        public object SystemMessageSerializer_Serialize_Recreate()
        {
            var payload = MessageSerializer.Serialize(System, null, TestRecreate);
            return MessageSerializer.Deserialize(System, payload);
        }

        [Benchmark]
        public object SystemMessageSerializer_Serialize_Supervise()
        {
            var payload = MessageSerializer.Serialize(System, null, TestSupervise);
            return MessageSerializer.Deserialize(System, payload);
        }

        [Benchmark]
        public object SystemMessageSerializer_Serialize_Watch()
        {
            var payload = MessageSerializer.Serialize(System, null, TestWatch);
            return MessageSerializer.Deserialize(System, payload);
        }

        [Benchmark]
        public object SystemMessageSerializer_Serialize_Unwatch()
        {
            var payload = MessageSerializer.Serialize(System, null, TestUnwatch);
            return MessageSerializer.Deserialize(System, payload);
        }

        //[Benchmark]
        public object SystemMessageSerializer_Serialize_Failed()
        {
            var payload = MessageSerializer.Serialize(System, null, TestFailed);
            return MessageSerializer.Deserialize(System, payload);
        }

        [Benchmark]
        public object SystemMessageSerializer_Serialize_DeathWatchNotification()
        {
            var payload = MessageSerializer.Serialize(System, null, TestDeathWatchNotification);
            return MessageSerializer.Deserialize(System, payload);
        }

        [Benchmark]
        public object SystemMessageSerializer_Serialize_Identify()
        {
            var payload = MessageSerializer.Serialize(System, null, TestIdentify);
            return MessageSerializer.Deserialize(System, payload);
        }

        [Benchmark]
        public object SystemMessageSerializer_Serialize_ActorIdentity()
        {
            var payload = MessageSerializer.Serialize(System, null, TestActorIdentity);
            return MessageSerializer.Deserialize(System, payload);
        }

        [Benchmark]
        public object SystemMessageSerializer_Serialize_ActorRef()
        {
            var payload = MessageSerializer.Serialize(System, null, LocalActorRef);
            return MessageSerializer.Deserialize(System, payload);
        }

        [Benchmark]
        public object SystemMessageSerializer_Serialize_Config()
        {
            var payload = MessageSerializer.Serialize(System, null, TestConfig);
            return MessageSerializer.Deserialize(System, payload);
        }

        [Benchmark]
        public object SystemMessageSerializer_Serialize_FromConfig()
        {
            var payload = MessageSerializer.Serialize(System, null, TestFromConfig);
            return MessageSerializer.Deserialize(System, payload);
        }

        [Benchmark]
        public object SystemMessageSerializer_Serialize_RoundRobinPool()
        {
            var payload = MessageSerializer.Serialize(System, null, TestRoundRobinPool);
            return MessageSerializer.Deserialize(System, payload);
        }

        [Benchmark]
        public object SystemMessageSerializer_Serialize_BroadcastPool()
        {
            var payload = MessageSerializer.Serialize(System, null, TestBroadcastPool);
            return MessageSerializer.Deserialize(System, payload);
        }

        [Benchmark]
        public object SystemMessageSerializer_Serialize_RandomPool()
        {
            var payload = MessageSerializer.Serialize(System, null, TestRandomPool);
            return MessageSerializer.Deserialize(System, payload);
        }

        [Benchmark]
        public object SystemMessageSerializer_Serialize_ScatterGatherFirstCompletedPool()
        {
            var payload = MessageSerializer.Serialize(System, null, TestScatterGatherFirstCompletedPool);
            return MessageSerializer.Deserialize(System, payload);
        }

        [Benchmark]
        public object SystemMessageSerializer_Serialize_TailChoppingPool()
        {
            var payload = MessageSerializer.Serialize(System, null, TestTailChoppingPool);
            return MessageSerializer.Deserialize(System, payload);
        }

        [Benchmark]
        public object SystemMessageSerializer_Serialize_RemoteRouterConfig()
        {
            var payload = MessageSerializer.Serialize(System, null, RemoteRouterConfig);
            return MessageSerializer.Deserialize(System, payload);
        }

        [Benchmark]
        public object SystemMessageSerializer_Serialize_ConsistentHashingPool()
        {
            var payload = MessageSerializer.Serialize(System, null, TestConsistentHashingPool);
            return MessageSerializer.Deserialize(System, payload);
        }
    }
}
