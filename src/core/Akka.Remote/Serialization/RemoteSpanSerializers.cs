using System;
using Google.Protobuf;
using Akka.Serialization;
using Akka.Actor;
using Akka.Routing;

namespace Akka.Remote.Serialization
{
    public static class ProtobufExtensions
    {
        //
        // CodedInputStream class
        //

        // TODO: should be a constructor
        public static CodedInputStream CreateCodedInputStream(ReadOnlySpan<byte> buffer)
        {
            return new CodedInputStream(buffer.ToArray());
        }

        //
        // CodedOutputStream class
        //

        // TODO: should be a constructor
        public static CodedOutputStream CreateCodedOutputStream(Span<byte> buffer)
        {
            return new CodedOutputStream(buffer.ToArray());
        }

        //
        // MessageExtensions class
        //

        // TODO: it still allocates
        public static void WriteTo(this IMessage message, Span<byte> buffer)
        {
            CodedOutputStream output = CreateCodedOutputStream(buffer);
            message.WriteTo(output);
            output.Flush();
        }

        // TODO: it still allocates
        public static void MergeFrom(this IMessage message, ReadOnlySpan<byte> bytes)
        {
            CodedInputStream input = CreateCodedInputStream(bytes);
            message.MergeFrom(input);
        }

        //
        // MessageParser<T> class
        //

        // TODO: it still allocates
        public static IMessage ParseFrom<T>(this MessageParser<T> parser, ReadOnlySpan<byte> bytes) where T : IMessage<T>, new()
        {
            T message = new T();
            message.MergeFrom(bytes);
            return message;
        }
    }

    public class ProtobufResizerSerializer : SpanSerializer
    {
        public ProtobufResizerSerializer(ExtendedActorSystem system) : base(system)
        {
        }

        public override int Identifier => 7;

        public override bool IncludeManifest => true;

        public override void ToBinary(object obj, out Span<byte> bytes)
        {
            switch (obj)
            {
                case null:
                    break;
                case DefaultResizer defaultResizer:
                    var message = new Proto.Msg.DefaultResizer();
                    message.LowerBound = (uint)defaultResizer.LowerBound;
                    message.UpperBound = (uint)defaultResizer.UpperBound;
                    message.PressureThreshold = (uint)defaultResizer.PressureThreshold;
                    message.RampupRate = defaultResizer.RampupRate;
                    message.BackoffThreshold = defaultResizer.BackoffThreshold;
                    message.BackoffRate = defaultResizer.BackoffRate;
                    message.MessagesPerResize = (uint)defaultResizer.MessagesPerResize;
                    message.WriteTo(bytes);
                    break;
                case var other:
                    throw new ArgumentException($"{nameof(ProtobufResizerSerializer)} only serializes DefaultResizer, not [{other.GetType().Name}]");
            }
        }

        public override object FromBinary(ReadOnlySpan<byte> bytes, Type manifest)
        {
            if (manifest == typeof(DefaultResizer))
            {
                var resizer = Proto.Msg.DefaultResizer.Parser.ParseFrom(bytes);
                return new DefaultResizer(
                    (int)resizer.LowerBound,
                    (int)resizer.UpperBound,
                    (int)resizer.PressureThreshold,
                    resizer.RampupRate,
                    resizer.BackoffThreshold,
                    resizer.BackoffRate,
                    (int)resizer.MessagesPerResize);
            }

            throw new ArgumentException($"{nameof(ProtobufResizerSerializer)} only deserializes DefaultResizer, not [{manifest.Name}]");
        }
    }
}
