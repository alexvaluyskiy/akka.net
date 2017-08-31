//-----------------------------------------------------------------------
// <copyright file="IMessage.cs" company="Akka.NET Project">
//     Copyright (C) 2009-2016 Lightbend Inc. <http://www.lightbend.com>
//     Copyright (C) 2013-2016 Akka.NET project <https://github.com/akkadotnet/akka.net>
// </copyright>
//-------------------

using System;
using Akka.Actor;
using Akka.Persistence.Serialization.Proto.Msg;
using Akka.Serialization;
using Akka.Util;
using Google.Protobuf;

namespace Akka.Persistence.Serialization
{
    public sealed class PersistenceSnapshotSerializer : Serializer
    {
        public PersistenceSnapshotSerializer(ExtendedActorSystem system) : base(system)
        {
        }

        public override bool IncludeManifest { get; } = true;

        public override byte[] ToBinary(object obj)
        {
            if (obj is Snapshot snap) return GetPersistentPayload(snap).ToByteArray();

            throw new ArgumentException($"Can't serialize object of type [{obj.GetType()}] in [{GetType()}]");
        }

        private PersistentPayload GetPersistentPayload(Snapshot snapshot)
        {
            Serializer serializer = system.Serialization.FindSerializerFor(snapshot);
            var payload = new PersistentPayload();

            if (serializer is SerializerWithStringManifest serializerManifest)
            {
                payload.PayloadManifest = ByteString.CopyFromUtf8(serializerManifest.Manifest(snapshot));
            }
            else
            {
                if (serializer.IncludeManifest)
                {
                    payload.PayloadManifest = ByteString.CopyFromUtf8(snapshot.GetType().TypeQualifiedName());
                }
            }

            payload.Payload = ByteString.CopyFrom(serializer.ToBinary(snapshot.Data));
            payload.SerializerId = serializer.Identifier;

            return payload;
        }

        public override object FromBinary(byte[] bytes, Type type)
        {
            if (type == typeof(Snapshot)) return GetSnapshot(bytes);

            throw new ArgumentException($"Unimplemented deserialization of message with type [{type}] in [{GetType()}]");
        }

        private Snapshot GetSnapshot(byte[] bytes)
        {
            var payload = PersistentPayload.Parser.ParseFrom(bytes);

            string manifest = "";
            if (payload.PayloadManifest != null) manifest = payload.PayloadManifest.ToStringUtf8();

            return new Snapshot(system.Serialization.Deserialize(payload.Payload.ToByteArray(), payload.SerializerId, manifest));
        }
    }
}