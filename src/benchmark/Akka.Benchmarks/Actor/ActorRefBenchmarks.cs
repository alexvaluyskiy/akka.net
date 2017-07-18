using System;
using System.Collections.Generic;
using System.Threading;
using Akka.Actor;
using BenchmarkDotNet.Attributes;
using Akka.Configuration;

namespace Akka.Benchmarks.Actor
{
    [Config(typeof(MyConfig))]
    [MemoryDiagnoser]
    public class ActorRefBenchmarks
    {
        private readonly ActorSystem System;
        private ManualResetEventSlim ResetEvent = new ManualResetEventSlim();

        public ActorRefBenchmarks()
        {
            var config = ConfigurationFactory.ParseString("akka.suppress-json-serializer-warning=true");
            System = ActorSystem.Create("RemoteSerializationBenchmarks", config);
        }

        [Benchmark]
        public IActorRef System_ActorOf()
        {
            return System.ActorOf(Props.Create<BenchmarkActor>(ResetEvent));
        }

        [Benchmark]
        public IActorRef System_ActorOf_with_name()
        {
            return System.ActorOf(Props.Create<BenchmarkActor>(ResetEvent), Guid.NewGuid().ToString());
        }

        [Benchmark]
        public bool System_ActorRef_Ask()
        {
            var actor = System.ActorOf(Props.Create<BenchmarkActor>(ResetEvent));
            return actor.Ask<bool>("ping").Result;
        }

        [Benchmark]
        public void System_ActorRef_Tell()
        {
            var actor = System.ActorOf(Props.Create<BenchmarkActor>(ResetEvent));
            actor.Tell(5);
            ResetEvent.Wait();
            ResetEvent.Reset();
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
                if (message is string)
                    Sender.Tell(true);

                if (message is int)
                    _resetEvent.Set();
            }
        }
    }
}
