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
    [BenchmarkCategory("actor")]
    public class ActorRefTellBenchmarks
    {
        private readonly ActorSystem System;
        private ManualResetEventSlim ResetEvent = new ManualResetEventSlim();
        private IActorRef TestActor;
        private ComplexType TestComplexType = new ComplexType("John Loke", 58);

        public ActorRefTellBenchmarks()
        {
            var config = ConfigurationFactory.ParseString("akka.suppress-json-serializer-warning=true");
            System = ActorSystem.Create("ActorRefTellBenchmarks", config);
            TestActor = System.ActorOf(Props.Create<BenchmarkActor>(ResetEvent));
        }

        [Params(1, 10)]
        public int N;

        [Benchmark]
        public void System_ActorRef_Tell()
        {
            for (int i = 0; i < N; i++)
            {
                TestActor.Tell(5);
                ResetEvent.Wait();
                ResetEvent.Reset();
            }
        }

        [Benchmark]
        public void System_ActorRef_Tell_ComplexType()
        {
            for (int i = 0; i < N; i++)
            {
                TestActor.Tell(TestComplexType);
                ResetEvent.Wait();
                ResetEvent.Reset();
            }
        }

        [Benchmark]
        public void System_ActorRef_Tell_ActorRef()
        {
            for (int i = 0; i < N; i++)
            {
                TestActor.Tell(TestActor);
                ResetEvent.Wait();
                ResetEvent.Reset();
            }
        }

        [Benchmark]
        public void System_ActorRef_Ask()
        {
            for (int i = 0; i < N; i++)
            {
                TestActor.Ask<string>("ping").Wait();
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
                if (message is string)
                    Sender.Tell("pong");

                if (message is int || message is ComplexType || message is IActorRef)
                    _resetEvent.Set();
            }
        }

        public class ComplexType
        {
            public ComplexType(string name, int age)
            {
                Name = name;
                Age = age;
            }

            public string Name { get; }

            public int Age { get; }
        }
    }
}
