using System;
using Akka.Configuration;
using BenchmarkDotNet.Attributes;
using Google.Protobuf;

namespace Akka.Benchmarks.Remote
{
    [Config(typeof(MyConfig))]
    [MemoryDiagnoser]
    [BenchmarkCategory("serialization")]
    public class ProtobufBenchmarks
    {
        private byte[] TestIntegerBytes = Convert.FromBase64String("xwEAAA==");

        [Benchmark]
        public ByteString ByteString_CopyFrom()
        {
            return ByteString.CopyFrom(TestIntegerBytes);
        }

        [Benchmark]
        public ByteString ByteString_CopyFromUtf8()
        {
            return ByteString.CopyFromUtf8("test");
        }
    }
}
