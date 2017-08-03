using Akka.Actor;
using Akka.Configuration;
using Akka.Remote;
using Akka.Remote.Configuration;
using BenchmarkDotNet.Attributes;

namespace Akka.Benchmarks.Remote.Serialization
{
    [Config(typeof(MyConfig))]
    [MemoryDiagnoser]
    [BenchmarkCategory("serialization")]
    public class MessageContainerSerializerBenchmarks
    {
        private readonly ActorSystem _system;
        private readonly ActorSelectionMessage _testActorSelectionMessage;

        public MessageContainerSerializerBenchmarks()
        {
            var config = ConfigurationFactory.ParseString("akka.suppress-json-serializer-warning=true").WithFallback(RemoteConfigFactory.Default());
            _system = ActorSystem.Create("MessageContainerSerializerBenchmarks", config);

            var pathElements = new SelectionPathElement[]
            {
                new SelectParent(), 
                new SelectChildName("test1"),
                new SelectChildPattern("test2"), 
            };
            _testActorSelectionMessage = new ActorSelectionMessage("some message", pathElements, true);
        }

        [Benchmark]
        public object MessageContainerSerializer_Serialize_ActorSelectionMessage()
        {
            var payload = MessageSerializer.Serialize(_system, null, _testActorSelectionMessage);
            return MessageSerializer.Deserialize(_system, payload);
        }
    }
}