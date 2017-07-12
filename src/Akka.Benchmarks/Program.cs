using System;
using BenchmarkDotNet.Running;

namespace Akka.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<ActorPathBenchmarks>();

            Console.ReadLine();
        }
    }
}