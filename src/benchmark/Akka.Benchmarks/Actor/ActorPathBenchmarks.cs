using System.Collections.Generic;
using Akka.Actor;
using BenchmarkDotNet.Attributes;

namespace Akka.Benchmarks.Actor
{
    [Config(typeof(MyConfig))]
    [MemoryDiagnoser]
    public class ActorPathBenchmarks
    {
        private static readonly ActorPath TestActorPath = ActorPath.Parse("akka.tcp://Sys@localhost:9091/user/foo");
        private static readonly RootActorPath RootAddress = new RootActorPath(Address.AllSystems);

        [Benchmark]
        public ActorPath ActorPath_Parse_local_address()
        {
            return ActorPath.Parse("akka://Sys/user/foo");
        }

        [Benchmark]
        public ActorPath ActorPath_Parse_remote_address()
        {
            return ActorPath.Parse("akka.tcp://Sys@localhost:9091/user/foo");
        }

        [Benchmark]
        public ActorPath ActorPath_TryParse_local_address()
        {
            ActorPath target;
            ActorPath.TryParse("akka://Sys/user/foo", out target);
            return target;
        }

        [Benchmark]
        public ActorPath ActorPath_TryParse_remote_address()
        {
            ActorPath target;
            ActorPath.TryParse("akka.tcp://Sys@localhost:9091/user/foo", out target);
            return target;
        }

        [Benchmark]
        public Address ActorPath_TryParseAddress_local_address()
        {
            Address target;
            ActorPath.TryParseAddress("akka://Sys/user/foo", out target);
            return target;
        }

        [Benchmark]
        public Address ActorPath_TryParseAddress_remote_address()
        {
            Address target;
            ActorPath.TryParseAddress("akka.tcp://Sys@localhost:9091/user/foo", out target);
            return target;
        }

        [Benchmark]
        public IReadOnlyList<string> ActorPath_Elements()
        {
            return TestActorPath.Elements;
        }

        [Benchmark]
        public ActorPath ActorPath_Concat_Operator()
        {
            return (RootAddress / "user" / "foo");
        }
		
		[Benchmark]
        public string ActorPath_ToStringWithAddress()
        {
            return TestActorPath.ToStringWithAddress();
        }
		
		[Benchmark]
        public string ActorPath_ToSerializationFormat()
        {
            return TestActorPath.ToSerializationFormat();
        }

        [Benchmark]
        public bool ActorPath_Equals()
        {
            return TestActorPath.Equals(TestActorPath);
        }

        [Benchmark]
        public int ActorPath_GetHashCode()
        {
            return TestActorPath.GetHashCode();
        }
    }
}
