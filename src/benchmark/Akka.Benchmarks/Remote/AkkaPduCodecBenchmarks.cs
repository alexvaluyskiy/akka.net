using System.Collections.Generic;
using Akka.Actor;
using Akka.Configuration;
using Akka.Remote.Configuration;
using BenchmarkDotNet.Attributes;
using Akka.Remote;
using Akka.Remote.Transport;
using Google.Protobuf;
using SerializedMessage = Akka.Remote.Serialization.Proto.Msg.Payload;

namespace Akka.Benchmarks.Remote
{
    [Config(typeof(MyConfig))]
    [MemoryDiagnoser]
    [BenchmarkCategory("serialization")]
    public class AkkaPduCodecBenchmarks
    {
        private readonly ActorSystem System;
        private Address TestLocalAddress = new Address("akka.tcp", nameof(AkkaPduCodec), "test", 5000);
        private IActorRef TestActorRef;
        private AkkaPduProtobuffCodec TestAkkaPduProtobuffCodec;
        private SeqNo TestSeqNo = new SeqNo(555);
        private Ack TestAck;
        private HandshakeInfo TestHandshake;
        private RemoteActorRefProvider TestRemoteActorRefProvider;
        private ByteString TestEncodedMessage;

        private SerializedMessage TestPayload = new SerializedMessage
        {
            Message = Google.Protobuf.ByteString.CopyFromUtf8("test"),
            SerializerId = 17,
            MessageManifest = Google.Protobuf.ByteString.CopyFromUtf8("System.String")
        };

        public AkkaPduCodecBenchmarks()
        {
            var config = ConfigurationFactory.ParseString(@"
                akka.suppress-json-serializer-warning=true
                akka.actor.provider = remote
                akka.remote.dot-netty.tcp = {
                    port = 0
                    hostname = localhost
                }
                akka.loglevel = WARNING
            ").WithFallback(RemoteConfigFactory.Default());
            System = ActorSystem.Create(nameof(AkkaPduCodec), config);
            TestRemoteActorRefProvider = RARP.For(System).Provider;

            TestActorRef = System.ActorOf(Props.Empty);
            TestAkkaPduProtobuffCodec = new AkkaPduProtobuffCodec();
            TestAck = new Ack(TestSeqNo, new List<SeqNo> { TestSeqNo });
            TestHandshake = new HandshakeInfo(TestLocalAddress, 555);
            TestEncodedMessage = TestAkkaPduProtobuffCodec.ConstructMessage(TestLocalAddress, TestActorRef, TestPayload, TestActorRef, TestSeqNo, TestAck);
        }

        [Benchmark]
        public ByteString AkkaPduProtobuffCodec_ConstructMessage()
        {
            return TestAkkaPduProtobuffCodec.ConstructMessage(TestLocalAddress, TestActorRef, TestPayload);
        }

        [Benchmark]
        public ByteString AkkaPduProtobuffCodec_ConstructMessage_with_sender_ack_seqno()
        {
            return TestAkkaPduProtobuffCodec.ConstructMessage(TestLocalAddress, TestActorRef, TestPayload, TestActorRef, TestSeqNo, TestAck);
        }

        [Benchmark]
        public ByteString AkkaPduProtobuffCodec_ConstructAssociate()
        {
            return TestAkkaPduProtobuffCodec.ConstructAssociate(TestHandshake);
        }

        [Benchmark]
        public ByteString AkkaPduProtobuffCodec_ConstructDisassociate()
        {
            return TestAkkaPduProtobuffCodec.ConstructDisassociate(DisassociateInfo.Quarantined);
        }

        [Benchmark]
        public ByteString AkkaPduProtobuffCodec_ConstructHeartbeat()
        {
            return TestAkkaPduProtobuffCodec.ConstructHeartbeat();
        }

        [Benchmark]
        public ByteString AkkaPduProtobuffCodec_ConstructPureAck()
        {
            return TestAkkaPduProtobuffCodec.ConstructPureAck(TestAck);
        }

        [Benchmark]
        public object AkkaPduProtobuffCodec_DecodeMessage()
        {
            return TestAkkaPduProtobuffCodec.DecodeMessage(TestEncodedMessage, TestRemoteActorRefProvider, TestLocalAddress);
        }
    }
}
