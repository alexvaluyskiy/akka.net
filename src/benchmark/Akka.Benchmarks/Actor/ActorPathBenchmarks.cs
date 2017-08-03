using System.Collections.Generic;
using Akka.Actor;
using BenchmarkDotNet.Attributes;

namespace Akka.Benchmarks.Actor
{
    [Config(typeof(MyConfig))]
    [MemoryDiagnoser]
    [BenchmarkCategory("actor")]
    public class ActorPathBenchmarks
    {
        private ActorPath TestActorPath = ActorPath.Parse("akka.tcp://Sys@localhost:9091/user/foo");
        private RootActorPath TestRootAddress = ActorPath.Parse("akka.tcp://Sys@localhost:9091/user/foo").Root as RootActorPath;
        private string TestLocalAddressString = "akka://Sys/user/foo";
        private string TestRemoteAddressString = "akka.tcp://Sys@localhost:9091/user/foo";
        private IEnumerable<string> TestActorElements = new List<string>() { "user", "foo" };
        private Address TestAddress = new Address("akka.tcp", "Sys", "localhost", 9091);

        [Benchmark]
        public ActorPath ActorPath_Parse_local_address()
        {
            return ActorPath.Parse(TestLocalAddressString);
        }

        [Benchmark]
        public ActorPath ActorPath_Parse_remote_address()
        {
            return ActorPath.Parse(TestRemoteAddressString);
        }

        [Benchmark]
        public ActorPath ActorPath_TryParse_local_address()
        {
            ActorPath target;
            ActorPath.TryParse(TestLocalAddressString, out target);
            return target;
        }

        [Benchmark]
        public ActorPath ActorPath_TryParse_remote_address()
        {
            ActorPath target;
            ActorPath.TryParse(TestRemoteAddressString, out target);
            return target;
        }

        [Benchmark]
        public Address ActorPath_TryParseAddress_local_address()
        {
            Address target;
            ActorPath.TryParseAddress(TestLocalAddressString, out target);
            return target;
        }

        [Benchmark]
        public Address ActorPath_TryParseAddress_remote_address()
        {
            Address target;
            ActorPath.TryParseAddress(TestRemoteAddressString, out target);
            return target;
        }

        [Benchmark]
        public ActorPath ActorPath_Concat_operator()
        {
            return (TestRootAddress / "user" / "foo");
        }

        [Benchmark]
        public ActorPath ActorPath_Concat_operator_with_enumerable()
        {
            return (TestRootAddress / TestActorElements);
        }

        [Benchmark]
        public string ActorPath_ToStringWithAddress()
        {
            return TestActorPath.ToStringWithAddress();
        }

        [Benchmark]
        public string ActorPath_ToStringWithoutAddress()
        {
            return TestActorPath.ToStringWithoutAddress();
        }

        [Benchmark]
        public string ActorPath_ToStringWithUid()
        {
            return TestActorPath.ToStringWithUid();
        }

        [Benchmark]
        public ActorPath ActorPath_Child()
        {
            return TestActorPath.Child("foo");
        }

        [Benchmark]
        public string ActorPath_ToSerializationFormat()
        {
            return TestActorPath.ToSerializationFormat();
        }

        [Benchmark]
        public string ActorPath_ToSerializationFormatWithAddress()
        {
            return TestActorPath.ToSerializationFormatWithAddress(TestAddress);
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

        [Benchmark]
        public IReadOnlyList<string> RootActorPath_Elements()
        {
            return TestRootAddress.Elements;
        }

        [Benchmark]
        public IReadOnlyList<string> ChildActorPath_Elements()
        {
            return TestActorPath.Elements;
        }

        [Benchmark]
        public IReadOnlyList<string> RootActorPath_ElementsWithUid()
        {
            return TestRootAddress.ElementsWithUid;
        }

        [Benchmark]
        public IReadOnlyList<string> ChildActorPath_ElementsWithUid()
        {
            return TestActorPath.ElementsWithUid;
        }

        [Benchmark]
        public ActorPath RootActorPath_Root()
        {
            return TestRootAddress.Root;
        }

        [Benchmark]
        public ActorPath ChildActorPath_Root()
        {
            return TestActorPath.Root;
        }

        [Benchmark]
        public int RootActorPath_CompareTo()
        {
            return TestRootAddress.CompareTo(TestRootAddress);
        }

        [Benchmark]
        public int ChildActorPath_CompareTo()
        {
            return TestActorPath.CompareTo(TestActorPath);
        }
    }
}
