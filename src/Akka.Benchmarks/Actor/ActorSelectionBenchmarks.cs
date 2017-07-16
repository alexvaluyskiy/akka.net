using System;
using System.Threading;
using Akka.Actor;
using Akka.Configuration;
using Akka.Remote.Configuration;
using Akka.Util.Internal;
using BenchmarkDotNet.Attributes;

namespace Akka.Benchmarks.Actor
{
    [Config(typeof(MyConfig))]
    [MemoryDiagnoser]
    public class ActorSelectionBenchmarks
    {
        private readonly ActorSystem System;
        private readonly ActorSelection TestActorSelection;
        private IActorRef TestSender;
        private ManualResetEventSlim ResetEvent = new ManualResetEventSlim();

        public ActorSelectionBenchmarks()
        {
            var config = ConfigurationFactory.ParseString("akka.suppress-json-serializer-warning=true");
            System = ActorSystem.Create("RemoteSerializationBenchmarks", config);
            System.ActorOf(Props.Create(() => new BenchmarkActor(ResetEvent)), "someactor");
            TestActorSelection = System.ActorSelection("akka://RemoteSerializationBenchmarks/user/someactor");
            TestSender = System.ActorOf(Props.Empty, "somesender");
        }

        [Benchmark]
        public ActorSelection ActorSelection_initialization_relative_path()
        {
            return System.ActorSelection("/user/someactor");
        }

        [Benchmark]
        public ActorSelection ActorSelection_initialization_absolute_path()
        {
            return System.ActorSelection("akka://RemoteSerializationBenchmarks/user/someactor");
        }

        [Benchmark]
        public void ActorSelection_tell()
        {
            TestActorSelection.Tell("the message");
            ResetEvent.Wait();
            ResetEvent.Reset();
        }

        [Benchmark]
        public void ActorSelection_tell_with_sender()
        {
            TestActorSelection.Tell("the message", TestSender);
            ResetEvent.Wait();
            ResetEvent.Reset();
        }

        [Benchmark]
        public IActorRef ActorSelection_local_ResolveOne()
        {
            return TestActorSelection.ResolveOne(TimeSpan.FromSeconds(3)).Result;
        }

        private class BenchmarkActor : UntypedActor
        {
            private readonly ManualResetEventSlim _resetEvent;

            public BenchmarkActor(ManualResetEventSlim resetEvent)
            {
                _resetEvent = resetEvent;
            }

            protected override void OnReceive(object message)
            {
                _resetEvent.Set();
            }
        }
    }
}
