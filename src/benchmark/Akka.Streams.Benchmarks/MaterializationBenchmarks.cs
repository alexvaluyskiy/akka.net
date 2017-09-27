using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using Akka.Streams;
using Akka.Streams.Dsl;
using BenchmarkDotNet.Attributes;

namespace Akka.Benchmarks.Actor
{
    [Config(typeof(MyConfig))]
    [MemoryDiagnoser]
    public class MaterializationBenchmarks
    {
        private ActorSystem _actorSystem;
        private ActorMaterializer _materializer;

        public MaterializationBenchmarks()
        {
        }

        [IterationSetup]
        public void IterationSetup()
        {
            Config config = @"
                akka.suppress-json-serializer-warning=true
            ";
            _actorSystem = ActorSystem.Create("ActorRefSpawnBenchmarks", config);
            _materializer = _actorSystem.Materializer();
        }

        [IterationCleanup]
        public void IterationCleanup()
        {
            _actorSystem.Terminate().Wait();
        }

        // MATERIALIZATION

        [Benchmark]
        public NotUsed Single_materialization_run()
        {
            return Source.Single(1).To(Sink.Ignore<int>()).Run(_materializer);
        }

        [Benchmark]
        public Task Single_materialization_runWith()
        {
            return Source.Single(1).RunWith(Sink.Ignore<int>(), _materializer);
        }

        // DEFAULT SOURCES

        [Benchmark]
        public Task Source_Empty_materialization()
        {
            return Source.Empty<int>().RunWith(Sink.Ignore<int>(), _materializer);
        }

        [Benchmark]
        public Task Source_Enumerator_materialization()
        {
            return Source.FromEnumerator(() => SimpleEnumerator()).RunWith(Sink.Ignore<int>(), _materializer);
        }

        [Benchmark]
        public Task Source_Enumerable_materialization()
        {
            return Source.From(SimpleEnumerable).RunWith(Sink.Ignore<int>(), _materializer);
        }

        // DEFAULT FLOWS

        [Benchmark]
        public Task Source_Single_with_Select_materialization()
        {
            return Source.Single(1).Select(c => c).RunWith(Sink.Ignore<int>(), _materializer);
        }

        [Benchmark]
        public Task Source_Single_with_Where_materialization()
        {
            return Source.Single(1).Where(c => c > 10).RunWith(Sink.Ignore<int>(), _materializer);
        }

        private static IEnumerable<int> SimpleEnumerable = new List<int>() { 1, 2, 3, 4, 5 };

        private static IEnumerator<int> SimpleEnumerator()
        {
            yield return 1;
            yield return 2;
            yield return 3;
            yield return 4;
            yield return 5;
        }
    }
}
