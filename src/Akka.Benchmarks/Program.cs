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
            BenchmarkRunner.Run<TypeExtensionsBenchmarks>();
            //BenchmarkRunner.Run<PrimitiveSerializersBenchmarks>();
            //BenchmarkRunner.Run<MiscMessageSerializerBenchmarks>();
            //BenchmarkRunner.Run<SystemMessageSerializerBenchmarks>();

            Console.ReadLine();
        }
    }
}