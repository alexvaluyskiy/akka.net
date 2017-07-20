using System;
using System.Collections.Generic;
using System.Threading;
using Akka.Actor;
using Akka.Configuration;
using BenchmarkDotNet.Attributes;

namespace Akka.Benchmarks.Actor
{
    [Config(typeof(MyConfig))]
    [MemoryDiagnoser]
    [BenchmarkCategory("actor")]
    public class InboxBenchmarks
    {
        private readonly ActorSystem System;
        private readonly ManualResetEventSlim ResetEvent = new ManualResetEventSlim();
        private readonly IActorRef TestActor;
        private readonly Inbox TestInbox;

        public InboxBenchmarks()
        {
            var config = ConfigurationFactory.ParseString("akka.suppress-json-serializer-warning=true");
            System = ActorSystem.Create("InboxBenchmarks", config);
            TestActor = System.ActorOf(Props.Create<BenchmarkActor>(ResetEvent));
            TestInbox = Inbox.Create(System);
        }

        [Benchmark]
        public Inbox Inbox_Create()
        {
            return Inbox.Create(System);
        }

        [Benchmark]
        public void Inbox_Send()
        {
            TestInbox.Send(TestActor, "hello");
            ResetEvent.Wait();
            ResetEvent.Reset();
        }

        [Benchmark]
        public void Inbox_WatchAndTell()
        {
            TestInbox.Watch(TestActor);
            TestActor.Tell("hello");
            ResetEvent.Wait();
            ResetEvent.Reset();
            TestInbox.Unwatch(TestActor);
        }

        [Benchmark]
        public object Inbox_Receive()
        {
            TestInbox.Send(TestActor, "ping");
            return TestInbox.Receive();
        }

        [Benchmark]
        public object Inbox_ReceiveTimeout()
        {
            TestInbox.Send(TestActor, "ping");
            return TestInbox.Receive(TimeSpan.FromMilliseconds(50));
        }

        [Benchmark]
        public object Inbox_ReceiveWhere()
        {
            TestInbox.Send(TestActor, "ping");
            return TestInbox.ReceiveWhere(c => c.Equals("pong"));
        }

        [Benchmark]
        public object Inbox_ReceiveWhereTimeout()
        {
            TestInbox.Send(TestActor, "ping");
            return TestInbox.ReceiveWhere(c => c.Equals("pong"), TimeSpan.FromMilliseconds(50));
        }

        [Benchmark]
        public object Inbox_ReceiveAsync()
        {
            TestInbox.Send(TestActor, "ping");
            return TestInbox.ReceiveAsync().GetAwaiter().GetResult();
        }

        [Benchmark]
        public object Inbox_ReceiveAsyncTimeout()
        {
            TestInbox.Send(TestActor, "ping");
            return TestInbox.ReceiveAsync(TimeSpan.FromMilliseconds(50)).GetAwaiter().GetResult();
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
                if (message.Equals("hello"))
                    _resetEvent.Set();
                else if (message.Equals("ping"))
                    Sender.Tell("pong");
            }
        }
    }
}
