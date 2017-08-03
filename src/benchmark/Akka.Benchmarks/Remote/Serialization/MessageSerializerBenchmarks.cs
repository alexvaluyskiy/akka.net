using System;
using Akka.Actor;
using Akka.Configuration;
using Akka.Remote;
using Akka.Remote.Configuration;
using Akka.Remote.Serialization;
using Akka.Remote.Serialization.Proto.Msg;
using BenchmarkDotNet.Attributes;

namespace Akka.Benchmarks.Remote.Serialization
{
    [Config(typeof(MyConfig))]
    [MemoryDiagnoser]
    [BenchmarkCategory("serialization")]
    public class MessageSerializerBenchmarks
    {
        private readonly ActorSystem System;
        private readonly PrimitiveSerializers BaseSerializer;
        private Address TestAddress = new Address("akka.tcp", nameof(MessageSerializerBenchmarks), "test", 5000);
        private Payload TestPayload = new Payload
        {
            Message = Google.Protobuf.ByteString.CopyFromUtf8("test"),
            SerializerId = 17,
            MessageManifest = Google.Protobuf.ByteString.CopyFromUtf8("System.String")
        };

        private ComplexType TestComplex = new ComplexType("John", 60);

        private Payload TestComplexPayload = new Payload
        {
            Message = Google.Protobuf.ByteString.FromBase64(
                "eyIkaWQiOiIxIiwiJHR5cGUiOiJBa2thLkJlbmNobWFya3MuUmVtb3RlLlNlcmlhbGl6YXRpb24uTWVzc2FnZVNlcmlhbGl6ZXJCZW5jaG1hcmtzK0NvbXBsZXhUeXBlLCBBa2thLkJlbmNobWFya3MiLCJOYW1lIjoiSm9obiIsIkFnZSI6eyIkIjoiSTYwIn19"),
            SerializerId = 1
        };

        public MessageSerializerBenchmarks()
        {
            var config = ConfigurationFactory.ParseString("akka.suppress-json-serializer-warning=true").WithFallback(RemoteConfigFactory.Default());
            System = ActorSystem.Create(nameof(MessageSerializerBenchmarks), config);
        }

        [Benchmark]
        public object MessageSerializer_Serialize_Primitive()
        {
            return MessageSerializer.Serialize(System, TestAddress, "test");
        }

        [Benchmark]
        public object MessageSerializer_Deserialize_Primitive()
        {
            return MessageSerializer.Deserialize(System, TestPayload);
        }

        [Benchmark]
        public object MessageSerializer_Serialize_Complex()
        {
            return MessageSerializer.Serialize(System, TestAddress, TestComplex);
        }

        [Benchmark]
        public object MessageSerializer_Deserialize_Complex()
        {
            return MessageSerializer.Deserialize(System, TestComplexPayload);
        }
    }
}
