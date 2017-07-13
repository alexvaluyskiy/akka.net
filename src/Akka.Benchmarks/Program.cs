using System;
using BenchmarkDotNet.Running;
using System.Threading.Tasks;
using Akka.Actor;

namespace Akka.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            //BenchmarkRunner.Run<ActorPathBenchmarks>();
            BenchmarkRunner.Run<RemoteSerializationBenchmarks>();

            Console.ReadLine();
        }
    }
}