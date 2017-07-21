using System;
using System.Collections.Generic;
using System.Threading;
using Akka.Actor;
using Akka.Configuration;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Engines;

namespace Akka.Benchmarks.Actor
{
    // TODO: in a progress
    [Config(typeof(MyConfig))]
    [MemoryDiagnoser]
    public abstract class FsmBenchmarks
    {
        private ActorSystem System;
        private IActorRef TestActor;
        private readonly ManualResetEventSlim ResetEvent = new ManualResetEventSlim();

        public FsmBenchmarks()
        {
            var config = ConfigurationFactory.ParseString("akka.suppress-json-serializer-warning=true");
            System = ActorSystem.Create("FsmBenchmarks", config);
            TestActor = System.ActorOf(Props.Create<BenchmarkFsm>(ResetEvent));
        }

        [Benchmark]
        public void Fsm_update_initial_state()
        {
            TestActor.Tell("reset");
            TestActor.Tell("staywithstate");
            ResetEvent.Wait();
            ResetEvent.Reset();
        }

        [Benchmark]
        public void Fsm_update_go_to_new_state()
        {
            TestActor.Tell("reset");
            TestActor.Tell("gotowork");
            ResetEvent.Wait();
            ResetEvent.Reset();
        }

        [Benchmark]
        public string Fsm_update_getstate()
        {
            TestActor.Tell("reset");
            TestActor.Tell("gotowork2");
            return TestActor.Ask<string>("getstate").Result;
        }

        private enum BenchmarkState
        {
            Init,
            Working
        }

        private class BenchmarkFsm : FSM<BenchmarkState, string>
        {
            private readonly ManualResetEventSlim _resetEvent;

            public BenchmarkFsm(ManualResetEventSlim resetEvent)
            {
                _resetEvent = resetEvent;

                StartWith(BenchmarkState.Init, string.Empty);

                When(BenchmarkState.Init, state =>
                {
                    if (state.FsmEvent.Equals("staywithstate"))
                    {
                        var newState = Stay().Using("updated");
                        _resetEvent.Set();
                        return newState;
                    }

                    if (state.FsmEvent.Equals("gotowork"))
                    {
                        var newState = GoTo(BenchmarkState.Working).Using("somenewstate");
                        _resetEvent.Set();
                        return newState;
                    }

                    if (state.FsmEvent.Equals("gotowork2"))
                    {
                        return GoTo(BenchmarkState.Working).Using("somenewstate2");
                    }

                    if (state.FsmEvent.Equals("reset"))
                    {
                        return GoTo(BenchmarkState.Init).Using(string.Empty);
                    }

                    return Stay();
                });

                When(BenchmarkState.Working, state =>
                {
                    if (state.FsmEvent.Equals("getstate"))
                    {
                        Sender.Tell(StateData);
                        return Stay();
                    }

                    if (state.FsmEvent.Equals("reset"))
                    {
                        return GoTo(BenchmarkState.Init).Using(string.Empty);
                    }

                    return Stay();
                });
            }
        }
    }
}
