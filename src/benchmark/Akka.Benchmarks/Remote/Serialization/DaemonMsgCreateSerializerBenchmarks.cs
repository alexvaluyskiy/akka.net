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
    public class DaemonMsgCreateSerializerBenchmarks
    {
        private readonly ActorSystem _system;
        private readonly DaemonMsgCreate _testDaemonMsgCreate;

        public DaemonMsgCreateSerializerBenchmarks()
        {
            var config = ConfigurationFactory.ParseString("akka.suppress-json-serializer-warning=true").WithFallback(RemoteConfigFactory.Default());
            _system = ActorSystem.Create("MessageContainerSerializerBenchmarks", config);

            var testActor = _system.ActorOf(Props.Empty);
            _testDaemonMsgCreate = new DaemonMsgCreate(Props.Empty, Deploy.Local, "user/some", testActor);
        }

        [Benchmark]
        public object DaemonMsgCreateSerializer_Serialize_DaemonMsgCreate()
        {
            var payload = MessageSerializer.Serialize(_system, null, _testDaemonMsgCreate);
            return MessageSerializer.Deserialize(_system, payload);
        }
    }
}