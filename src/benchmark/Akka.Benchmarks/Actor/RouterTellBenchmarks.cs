using Akka.Actor;
using BenchmarkDotNet.Attributes;
using System.Threading;
using Akka.Routing;
using Akka.Configuration;

namespace Akka.Benchmarks.Actor
{
    [Config(typeof(MyConfig))]
    [MemoryDiagnoser]
    [BenchmarkCategory("actor")]
    public class RouterTellBenchmarks
    {
        private ActorSystem System;
        private readonly ManualResetEventSlim ResetEvent = new ManualResetEventSlim();
        private readonly IActorRef TestRoundRobinPool;
        private readonly IActorRef TestRoundRobinGroup;

        public RouterTellBenchmarks()
        {
            var config = ConfigurationFactory.ParseString("akka.suppress-json-serializer-warning=true");
            System = ActorSystem.Create("RouterBenchmarks", config);
            TestRoundRobinPool = System.ActorOf(Props.Create<BenchmarkActor>(ResetEvent).WithRouter(new RoundRobinPool(2)));
            System.ActorOf(Props.Create<BenchmarkActor>(ResetEvent), "worker1");
			System.ActorOf(Props.Create<BenchmarkActor>(ResetEvent), "worker2");
            TestRoundRobinGroup = System.ActorOf(Props.Empty.WithRouter(new RoundRobinGroup("/user/worker1", "/user/worker2")));
        }

        [Params(1)]
        public int N;

        [Benchmark]
        public void Router_RoundRobinPool_Tell()
        {
            for (int i = 0; i < N; i++)
            {
                TestRoundRobinPool.Tell("TestRoundRobinPoolOne");
                ResetEvent.Wait();
                ResetEvent.Reset();
            }
        }

        [Benchmark]
        public void Router_RoundRobinGroup_Tell()
        {
            for (int i = 0; i < N; i++)
            {
                TestRoundRobinGroup.Tell("TestRoundRobinPoolOne");
                ResetEvent.Wait();
                ResetEvent.Reset();
            }
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
