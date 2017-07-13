using System.Collections.Generic;
using Akka.Actor;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Exporters;
using Akka.Remote.Serialization;
using Akka.Configuration;

namespace Akka.Benchmarks
{
    [Config(typeof(MyConfig))]
    [MemoryDiagnoser]
    public class RemoteSerializationBenchmarks
    {
        private readonly ActorSystem System = ActorSystem.Create("RemoteSerializationBenchmarks");
        private readonly PrimitiveSerializers PrimitiveSerializer;
        private readonly MiscMessageSerializer MiscMessageSerializer;

        public RemoteSerializationBenchmarks()
        {
            var config = ConfigurationFactory.ParseString("akka.suppress-json-serializer-warning = true");
            System = ActorSystem.Create("RemoteSerializationBenchmarks", config);
            PrimitiveSerializer = new PrimitiveSerializers(System as ExtendedActorSystem);
            MiscMessageSerializer = new MiscMessageSerializer(System as ExtendedActorSystem);
        }

        [Benchmark]
        public int PrimitiveSerializers_Parse_Int32()
        {
            var bytes = PrimitiveSerializer.ToBinary(455);
            return PrimitiveSerializer.FromBinary<int>(bytes);
        }

        [Benchmark]
        public long PrimitiveSerializers_Parse_Int64()
        {
            var bytes = PrimitiveSerializer.ToBinary(455L);
            return PrimitiveSerializer.FromBinary<long>(bytes);
        }

        [Benchmark]
        public string PrimitiveSerializers_Parse_String()
        {
            var bytes = PrimitiveSerializer.ToBinary("Some Simple String");
            return PrimitiveSerializer.FromBinary<string>(bytes);
        }

        [Benchmark]
        public Identify MiscMessageSerializer_Parse_Identify()
        {
            var identify = new Identify("test");
            var bytes = MiscMessageSerializer.ToBinary(identify);
            var manifest = MiscMessageSerializer.Manifest(identify);
            return (Identify)MiscMessageSerializer.FromBinary(bytes, manifest);
        }

        [Benchmark]
        public ActorIdentity MiscMessageSerializer_Parse_ActorIdentity()
        {
            var actorIdentify = new ActorIdentity("test", null);
            var bytes = MiscMessageSerializer.ToBinary(actorIdentify);
            var manifest = MiscMessageSerializer.Manifest(actorIdentify);
            return (ActorIdentity)MiscMessageSerializer.FromBinary(bytes, manifest);
        }
    }
}
