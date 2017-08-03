using System;
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
    public class PrimitiveSerializersBenchmarks
    {
        private readonly ActorSystem _system;
        private Exception TestException;
        private Exception TestExceptionWithInner;

        public PrimitiveSerializersBenchmarks()
        {
            var config = ConfigurationFactory.ParseString("akka.suppress-json-serializer-warning=true").WithFallback(RemoteConfigFactory.Default());
            _system = ActorSystem.Create("PrimitiveSerializersBenchmarks", config);

            try
            {
                throw new ActorInitializationException("test");
            }
            catch (Exception ex)
            {
                TestException = ex;
            }

            try
            {
                throw new ActorInitializationException("test", new ArgumentException("bug"));
            }
            catch (Exception ex)
            {
                TestExceptionWithInner = ex;
            }
        }

        [Benchmark]
        public object PrimitiveSerializers_Serialize_Int32()
        {
            var payload = MessageSerializer.Serialize(_system, null, 455);
            return MessageSerializer.Deserialize(_system, payload);
        }

        [Benchmark]
        public object PrimitiveSerializers_Serialize_Int64()
        {
            var payload = MessageSerializer.Serialize(_system, null, 455L);
            return MessageSerializer.Deserialize(_system, payload);
        }

        [Benchmark]
        public object PrimitiveSerializers_Serialize_String()
        {
            var payload = MessageSerializer.Serialize(_system, null, "test");
            return MessageSerializer.Deserialize(_system, payload);
        }

        //[Benchmark]
        public object PrimitiveSerializers_Serialize_Exception()
        {
            var payload = MessageSerializer.Serialize(_system, null, TestException);
            return MessageSerializer.Deserialize(_system, payload);
        }

        //[Benchmark]
        public object PrimitiveSerializers_Serialize_Exception_With_InnerException()
        {
            var payload = MessageSerializer.Serialize(_system, null, TestExceptionWithInner);
            return MessageSerializer.Deserialize(_system, payload);
        }
    }
}
