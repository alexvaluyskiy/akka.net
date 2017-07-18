using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text.RegularExpressions;
using Akka.Actor;
using Akka.Util;
using BenchmarkDotNet.Attributes;

namespace Akka.Benchmarks
{
    [Config(typeof(MyConfig))]
    [MemoryDiagnoser]
    public class TypeExtensionsBenchmarks
    {
        private Type StandardType = typeof(string);
        private Type NonStandardType = typeof(IActorRef);

        [Params(1, 100)]
        public int N;

        [Benchmark]
        public void Type_TypeQualifiedName_CoreLib_type()
        {
            for (int i = 0; i < N; i++)
            {
                var a = StandardType.TypeQualifiedName();
            }
        }

        [Benchmark]
        public void Type_TypeQualifiedName_Custom_type()
        {
            for (int i = 0; i < N; i++)
            {
                var a = NonStandardType.TypeQualifiedName();
            }
        }
    }
}
