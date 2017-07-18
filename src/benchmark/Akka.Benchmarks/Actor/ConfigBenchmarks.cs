using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Configuration;
using Akka.Configuration.Hocon;
using BenchmarkDotNet.Attributes;

namespace Akka.Benchmarks.Actor
{
    [Config(typeof(MyConfig))]
    [MemoryDiagnoser]
    public class ConfigBenchmarks
    {
        private Config TestConfiguration { get; } = @"
            akka {
                string-config = ""1.3.0""
                boolean-config = on
                int-config = 1244
                long-config = 4353454
                float-config = 46.5
                timespan-config = 10s
                string-list-config = [""Akka.Event.DefaultLogger"", ""Akka.Event.TraceLogger""]
                actor {
                    provider = ""Akka.Actor.LocalActorRefProvider""
                    creation-timeout = 20s
                }
                substitution-config = ${akka.string-config}
                substitution-concat-config = Hello ${akka.string-config}
            }";


        [Benchmark(Baseline = true)]
        public HoconValue Config_GetValue()
        {
            return TestConfiguration.GetValue("akka.boolean-config");
        }

        [Benchmark]
        public bool Config_GetBoolean()
        {
            return TestConfiguration.GetBoolean("akka.boolean-config");
        }

        [Benchmark]
        public int Config_GetInt()
        {
            return TestConfiguration.GetInt("akka.int-config");
        }

        [Benchmark]
        public long Config_GetLong()
        {
            return TestConfiguration.GetLong("akka.long-config");
        }

        [Benchmark]
        public string Config_GetString()
        {
            return TestConfiguration.GetString("akka.string-config");
        }

        [Benchmark]
        public float Config_GetFloat()
        {
            return TestConfiguration.GetFloat("akka.float-config");
        }

        [Benchmark]
        public double Config_GetDouble()
        {
            return TestConfiguration.GetDouble("akka.float-config");
        }

        [Benchmark]
        public decimal Config_GetDecimal()
        {
            return TestConfiguration.GetDecimal("akka.float-config");
        }

        [Benchmark]
        public TimeSpan Config_GetTimeSpan()
        {
            return TestConfiguration.GetTimeSpan("akka.timespan-config");
        }

        [Benchmark]
        public IList<string> Config_GetStringList()
        {
            return TestConfiguration.GetStringList("akka.string-list-config");
        }

        [Benchmark]
        public Config Config_GetConfig()
        {
            return TestConfiguration.GetConfig("akka.actor");
        }

        [Benchmark]
        public bool Config_HasPath()
        {
            return TestConfiguration.HasPath("akka.boolean-config");
        }

        [Benchmark]
        public string Config_ToString()
        {
            return TestConfiguration.ToString();
        }

        [Benchmark]
        public string Config_ToStringIncludeFallback()
        {
            return TestConfiguration.ToString(true);
        }

        [Benchmark]
        public Config Config_Empty()
        {
            return Config.Empty;
        }

        [Benchmark]
        public string Config_Substitution_GetString()
        {
            return TestConfiguration.GetString("akka.substitution-config");
        }

        [Benchmark]
        public string Config_Substitution_Concat_GetString()
        {
            return TestConfiguration.GetString("akka.substitution-concat-config");
        }
    }
}
