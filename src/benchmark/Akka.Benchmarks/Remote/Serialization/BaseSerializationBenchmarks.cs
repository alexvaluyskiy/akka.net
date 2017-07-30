using System;
using Akka.Actor;
using Akka.Configuration;
using Akka.Remote.Configuration;
using Akka.Remote.Serialization;
using BenchmarkDotNet.Attributes;

namespace Akka.Benchmarks.Remote.Serialization
{
    [Config(typeof(MyConfig))]
    [MemoryDiagnoser]
    [BenchmarkCategory("serialization")]
    public class BaseSerializationBenchmarks
    {
        private readonly ActorSystem System;
        private readonly PrimitiveSerializers BaseSerializer;
        private byte[] TestSerializedInteger = Convert.FromBase64String("xwEAAA==");
        private Address TestAddress = new Address("akka.tcp", "BaseSerializationBenchmarks", "test", 5000);

        public BaseSerializationBenchmarks()
        {
            var config = ConfigurationFactory.ParseString("akka.suppress-json-serializer-warning=true").WithFallback(RemoteConfigFactory.Default());
            System = ActorSystem.Create("BaseSerializationBenchmarks", config);
            BaseSerializer = new PrimitiveSerializers(System as ExtendedActorSystem);
        }

        [Benchmark]
        public int Serializer_Get_Identifier()
        {
            return BaseSerializer.Identifier;
        }

        [Benchmark]
        public byte[] Serializer_ToBinary()
        {
            return BaseSerializer.ToBinary(555);
        }

        [Benchmark]
        public byte[] Serializer_ToBinaryWithAddress()
        {
            return BaseSerializer.ToBinaryWithAddress(TestAddress, 555);
        }

        [Benchmark]
        public object Serializer_FromBinary()
        {
            return BaseSerializer.FromBinary(TestSerializedInteger, typeof(int));
        }

        [Benchmark]
        public int Serializer_FromBinary_Generic()
        {
            return BaseSerializer.FromBinary<int>(TestSerializedInteger);
        }
    }
}
