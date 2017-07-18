using System;
using Akka.Actor;
using BenchmarkDotNet.Attributes;
using Akka.Remote.Serialization;
using Akka.Configuration;
using Akka.Dispatch.SysMsg;
using Akka.Remote.Configuration;

namespace Akka.Benchmarks
{
    [Config(typeof(MyConfig))]
    [MemoryDiagnoser]
    public class SystemMessageSerializerBenchmarks
    {
        private readonly ActorSystem System;
        private readonly SystemMessageSerializer SystemMessageSerializer;

        IActorRef LocalActorRef { get; }
        Create TestCreate { get; }
        Recreate TestRecreate { get; } = new Recreate(new ArgumentException());
        Supervise TestSupervise { get; }
        Watch TestWatch { get; }
        Unwatch TestUnwatch { get; }
        Failed TestFailed { get; }
        DeathWatchNotification TestDeathWatchNotification { get; }

        public SystemMessageSerializerBenchmarks()
        {
            var config = ConfigurationFactory.ParseString("akka.suppress-json-serializer-warning=true").WithFallback(RemoteConfigFactory.Default());
            System = ActorSystem.Create("RemoteSerializationBenchmarks", config);
            SystemMessageSerializer = new SystemMessageSerializer(System as ExtendedActorSystem);
            LocalActorRef = System.ActorOf(Props.Empty);
            TestCreate = new Create(new ActorInitializationException(LocalActorRef, "message"));
            TestSupervise = new Supervise(LocalActorRef, true);
            TestWatch = new Watch((IInternalActorRef)LocalActorRef, (IInternalActorRef)LocalActorRef);
            TestUnwatch = new Unwatch((IInternalActorRef)LocalActorRef, (IInternalActorRef)LocalActorRef);
            TestFailed = new Failed(LocalActorRef, new ArgumentException(), 43534);
            TestDeathWatchNotification = new DeathWatchNotification(LocalActorRef, true, false);
        }

        //[Benchmark]
        public object SystemMessageSerializer_Serialize_Create()
        {
            var bytes = SystemMessageSerializer.ToBinary(TestCreate);
            return SystemMessageSerializer.FromBinary<Create>(bytes);
        }

        //[Benchmark]
        public object SystemMessageSerializer_Serialize_Recreate()
        {
            var bytes = SystemMessageSerializer.ToBinary(TestRecreate);
            return SystemMessageSerializer.FromBinary<Recreate>(bytes);
        }

        [Benchmark]
        public object SystemMessageSerializer_Serialize_Supervise()
        {
            var bytes = SystemMessageSerializer.ToBinary(TestSupervise);
            return SystemMessageSerializer.FromBinary<Supervise>(bytes);
        }

        [Benchmark]
        public object SystemMessageSerializer_Serialize_Watch()
        {
            var bytes = SystemMessageSerializer.ToBinary(TestWatch);
            return SystemMessageSerializer.FromBinary<Watch>(bytes);
        }

        [Benchmark]
        public object SystemMessageSerializer_Serialize_Unwatch()
        {
            var bytes = SystemMessageSerializer.ToBinary(TestUnwatch);
            return SystemMessageSerializer.FromBinary<Unwatch>(bytes);
        }

        //[Benchmark]
        public object SystemMessageSerializer_Serialize_Failed()
        {
            var bytes = SystemMessageSerializer.ToBinary(TestFailed);
            return SystemMessageSerializer.FromBinary<Failed>(bytes);
        }

        [Benchmark]
        public object SystemMessageSerializer_Serialize_DeathWatchNotification()
        {
            var bytes = SystemMessageSerializer.ToBinary(TestDeathWatchNotification);
            return SystemMessageSerializer.FromBinary<DeathWatchNotification>(bytes);
        }
    }
}
