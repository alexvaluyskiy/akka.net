﻿using Akka.Actor;
using BenchmarkDotNet.Attributes;
using Akka.Remote.Serialization;
using Akka.Configuration;
using Akka.Remote.Configuration;

namespace Akka.Benchmarks
{
    [Config(typeof(MyConfig))]
    [MemoryDiagnoser]
    public class PrimitiveSerializersBenchmarks
    {
        private readonly ActorSystem System;
        private readonly PrimitiveSerializers PrimitiveSerializer;
        private static Config TestConfig = RemoteConfigFactory.Default();

        public PrimitiveSerializersBenchmarks()
        {
            var config = ConfigurationFactory.ParseString("akka.suppress-json-serializer-warning=true").WithFallback(RemoteConfigFactory.Default());
            System = ActorSystem.Create("RemoteSerializationBenchmarks", config);
            PrimitiveSerializer = new PrimitiveSerializers(System as ExtendedActorSystem);
        }

        [Benchmark]
        public int PrimitiveSerializers_Serialize_Int32()
        {
            var bytes = PrimitiveSerializer.ToBinary(455);
            return PrimitiveSerializer.FromBinary<int>(bytes);
        }

        [Benchmark]
        public long PrimitiveSerializers_Serialize_Int64()
        {
            var bytes = PrimitiveSerializer.ToBinary(455L);
            return PrimitiveSerializer.FromBinary<long>(bytes);
        }

        [Benchmark]
        public string PrimitiveSerializers_Serialize_String()
        {
            var bytes = PrimitiveSerializer.ToBinary("Some Simple String");
            return PrimitiveSerializer.FromBinary<string>(bytes);
        }
    }
}
