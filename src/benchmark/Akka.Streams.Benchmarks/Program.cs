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
            BenchmarkRunner.Run<MaterializationBenchmarks>();

            Console.ReadLine();
        }
    }
}