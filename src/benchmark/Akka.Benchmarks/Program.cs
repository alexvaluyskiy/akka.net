using System;
using BenchmarkDotNet.Running;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Benchmarks.Actor;
using Akka.Benchmarks.Remote.Serialization;

namespace Akka.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<ActorSystemBenchmarks>();

            BenchmarkRunner.Run<RouterTellBenchmarks>();
            BenchmarkRunner.Run<ActorRefTellBenchmarks>();
            BenchmarkRunner.Run<ActorRefSpawnBenchmarks>();

            BenchmarkRunner.Run<InboxBenchmarks>();
            BenchmarkRunner.Run<ConfigBenchmarks>();

            BenchmarkRunner.Run<ActorSelectionBenchmarks>();
            BenchmarkRunner.Run<ActorPathBenchmarks>();
            BenchmarkRunner.Run<TypeExtensionsBenchmarks>();

            BenchmarkRunner.Run<SerializationBenchmarks>();
            BenchmarkRunner.Run<BaseSerializerBenchmarks>();
            BenchmarkRunner.Run<PrimitiveSerializersBenchmarks>();
            BenchmarkRunner.Run<MiscMessageSerializerBenchmarks>();
            BenchmarkRunner.Run<SystemMessageSerializerBenchmarks>();

            Console.ReadLine();
        }
    }
}