using System;
using Akka.Actor;
using Akka.Configuration;
using Akka.Remote.Configuration;
using BenchmarkDotNet.Attributes;
using Akka.Serialization;
using Akka.Remote;
using Akka.Util;

namespace Akka.Benchmarks.Actor
{
    [Config(typeof(MyConfig))]
    [MemoryDiagnoser]
    [BenchmarkCategory("serialization")]
    public class SerializationBenchmarks
    {
        private readonly ActorSystem System;
        private Serialization.Serialization TestSerialization;
        private byte[] TestIntegerBytes = Convert.FromBase64String("xwEAAA==");
        private IActorRef TestActorRef;

        public SerializationBenchmarks()
        {
            var config = ConfigurationFactory.ParseString(@"
                akka.suppress-json-serializer-warning=true
            ").WithFallback(RemoteConfigFactory.Default());
            System = ActorSystem.Create("SerializationBenchmarks", config);
            TestSerialization = new Serialization.Serialization(System as ExtendedActorSystem);
            TestActorRef = System.ActorOf(Props.Empty);
        }

        [Benchmark]
        public Serialization.Serialization Serialization_constructor()
        {
            return new Serialization.Serialization(System as ExtendedActorSystem);
        }

        [Benchmark]
        public Serializer Serialization_FindSerializerForType_preregistered()
        {
            return TestSerialization.FindSerializerForType(typeof(int));
        }

        [Benchmark]
        public Serializer Serialization_FindSerializerForType_subclass()
        {
            return TestSerialization.FindSerializerForType(typeof(RemoteActorRef));
        }

        [Benchmark]
        public Serializer Serialization_FindSerializerForType_ObjectSerializer()
        {
            return TestSerialization.FindSerializerForType(typeof(Uri));
        }

        [Benchmark]
        public object Serialization_Deserialize_type()
        {
            return TestSerialization.Deserialize(TestIntegerBytes, 17, typeof(int));
        }

        [Benchmark]
        public object Serialization_Deserialize_manifest_with_type()
        {
            return TestSerialization.Deserialize(TestIntegerBytes, 17, typeof(int).TypeQualifiedName());
        }

        //[Benchmark]
        public void Serialization_Deserialize_manifest_with_manifest()
        {
            // TODO: implement it
        }

        [Benchmark]
        public string Serialization_SerializedActorPath()
        {
            return Serialization.Serialization.SerializedActorPath(TestActorRef);
        }

        [Benchmark]
        public string Serialization_SerializedActorPath_NoBody()
        {
            return Serialization.Serialization.SerializedActorPath(ActorRefs.Nobody);
        }

        [Benchmark]
        public string Serialization_SerializedActorPath_NoSender()
        {
            return Serialization.Serialization.SerializedActorPath(ActorRefs.NoSender);
        }
    }
}
