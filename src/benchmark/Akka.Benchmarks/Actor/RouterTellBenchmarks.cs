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

        public RouterTellBenchmarks()
        {
            var config = ConfigurationFactory.ParseString("akka.suppress-json-serializer-warning=true");
            System = ActorSystem.Create("RouterBenchmarks", config);
            TestRoundRobinPool = System.ActorOf(Props.Create<BenchmarkActor>(ResetEvent).WithRouter(new RoundRobinPool(2)));
        }

        [Params(1, 10)]
        public int N;

        [Benchmark]
        public void Router_RoundRobinPool_One_Tell()
        {
            for (int i = 0; i < N; i++)
            {
                TestRoundRobinPool.Tell("TestRoundRobinPoolOne");
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
                if (message.Equals("TestRoundRobinPoolOne"))
                {
                    _resetEvent.Set();
                }
            }
        }
    }
}
