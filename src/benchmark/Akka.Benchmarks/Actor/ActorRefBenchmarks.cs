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

    [Config(typeof(MyConfig))]
    [MemoryDiagnoser]
    [BenchmarkCategory("actor")]
    public class ActorRefSpawnBenchmarks
    {
        private ActorSystem System;
        private Config config = ConfigurationFactory.ParseString("akka.suppress-json-serializer-warning=true");
        private IActorRef TestActor;

        [IterationSetup]
        public void IterationSetup()
        {
            System = ActorSystem.Create("ActorRefSpawnBenchmarks", config);
        }

        [IterationCleanup]
        public void IterationCleanup()
        {
            System.Terminate().Wait();
        }

        [Benchmark]
        public void System_ActorOf()
        {
            System.ActorOf(Props.Create<BenchmarkActor>());
        }

        [Benchmark]
        public void System_ActorOf_with_child()
        {
            System.ActorOf(BenchmarkWithChildActor.Props(1, 2));
        }

        private class BenchmarkActor : UntypedActor
        {
            protected override void OnReceive(object message)
            {
            }
        }

        private class BenchmarkWithChildActor : UntypedActor
        {
            private readonly int _position;
            private readonly int _maxPosition;

            public BenchmarkWithChildActor(int position, int maxPosition)
            {
                _position = position;
                _maxPosition = maxPosition;
            }

            protected override void PreStart()
            {
                if (_position < _maxPosition)
                {
                    Context.ActorOf(Props(_position + 1, _maxPosition));
                }
            }

            protected override void OnReceive(object message)
            {
            }

            public static Props Props(int position, int maxPosition) =>
                Akka.Actor.Props.Create<BenchmarkWithChildActor>(position, maxPosition);
        }
    }
}
