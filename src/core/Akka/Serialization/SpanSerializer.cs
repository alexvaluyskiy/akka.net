using System;
using Akka.Actor;

namespace Akka.Serialization
{
    public abstract class SpanSerializer
    {
        protected SpanSerializer(ExtendedActorSystem system)
        {
            System = system;
        }

        public ExtendedActorSystem System { get; }

        public abstract int Identifier { get; }

        public abstract bool IncludeManifest { get; }

        public abstract void ToBinary(object obj, out Span<byte> bytes);

        public object FromBinary(ReadOnlySpan<byte> bytes) => FromBinary(bytes, null);

        public abstract object FromBinary(ReadOnlySpan<byte> bytes, Type manifest);

        public virtual T FromBinary<T>(ReadOnlySpan<byte> bytes) => (T)FromBinary(bytes, typeof(T));
    }

    public class ByteArraySpanSerializer : SpanSerializer
    {
        public ByteArraySpanSerializer(ExtendedActorSystem system) : base(system)
        {
        }

        public override int Identifier { get; } = 5;

        public override bool IncludeManifest { get; } = false;

        public override void ToBinary(object obj, out Span<byte> bytes)
        {
            switch (obj)
            {
                case null:
                    break;
                case byte[] byteArray:
                    bytes = byteArray.AsSpan();
                    break;
                case var other:
                    throw new ArgumentException($"{nameof(ByteArraySpanSerializer)} only serializes byte arrays, not [{other.GetType().Name}]");
            }
        }

        public override object FromBinary(ReadOnlySpan<byte> bytes, Type manifest)
        {
            return bytes.ToArray();
        }
    }

    public class IntSerializer : SpanSerializer
    {
        public IntSerializer(ExtendedActorSystem system) : base(system)
        {
        }

        public override int Identifier { get; } = 6;

        public override bool IncludeManifest { get; } = false;

        public override void ToBinary(object obj, out Span<byte> bytes)
        {
            if (bytes.Length < 4)
                throw new ArgumentException("wrong Span size");

            var value = (int)obj;

            bytes[0] = (byte)(value >> 0);
            bytes[1] = (byte)(value >> 8);
            bytes[2] = (byte)(value >> 16);
            bytes[3] = (byte)(value >> 24);
        }

        public override object FromBinary(ReadOnlySpan<byte> bytes, Type manifest)
        {
            if (bytes.Length < 4)
                throw new ArgumentException("wrong Span size");

            int b = bytes[0] | bytes[1] << 8 | bytes[2] << 16 | bytes[3] << 24;
            return b;
        }
    }
}