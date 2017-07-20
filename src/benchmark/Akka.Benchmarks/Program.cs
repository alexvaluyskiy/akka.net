using System;
using BenchmarkDotNet.Running;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Benchmarks.Actor;

namespace Akka.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<RouterTellBenchmarks>();
            BenchmarkRunner.Run<ActorRefTellBenchmarks>();

            BenchmarkRunner.Run<InboxBenchmarks>();
            BenchmarkRunner.Run<ConfigBenchmarks>();

            BenchmarkRunner.Run<ActorSelectionBenchmarks>();
            BenchmarkRunner.Run<ActorPathBenchmarks>();
            BenchmarkRunner.Run<TypeExtensionsBenchmarks>();

            BenchmarkRunner.Run<PrimitiveSerializersBenchmarks>();
            BenchmarkRunner.Run<MiscMessageSerializerBenchmarks>();
            BenchmarkRunner.Run<SystemMessageSerializerBenchmarks>();

            Console.ReadLine();
        }
    }
}