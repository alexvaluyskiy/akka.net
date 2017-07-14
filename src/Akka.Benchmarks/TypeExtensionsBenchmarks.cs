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

        [Benchmark]
        public void Type_TypeQualifiedNameWithOptimized_CoreLib_type()
        {
            for (int i = 0; i < N; i++)
            {
                var a = StandardType.TypeQualifiedNameOptimized();
            }
        }

        [Benchmark]
        public void Type_TypeQualifiedNameOptimized_Custom_type()
        {
            for (int i = 0; i < N; i++)
            {
                var a = NonStandardType.TypeQualifiedNameOptimized();
            }
        }
    }

    public static class TypeExtensions
    {
        private static readonly ConcurrentDictionary<Type, string> ShortenedTypeNames = new ConcurrentDictionary<Type, string>();
        private static readonly string CoreAssemblyName = typeof(int).GetTypeInfo().Assembly.GetName().Name;

        public static string TypeQualifiedNameOptimized(this Type type)
        {
            string shortened;
            if (ShortenedTypeNames.TryGetValue(type, out shortened))
            {
                return shortened;
            }
            else
            {
                var assemblyName = type.GetTypeInfo().Assembly.GetName().Name;
                shortened = assemblyName.Equals(CoreAssemblyName)
                    ? type.GetTypeInfo().FullName
                    : $"{type.GetTypeInfo().FullName}, {assemblyName}";
                ShortenedTypeNames.TryAdd(type, shortened);
                return shortened;
            }
        }
    }
}
