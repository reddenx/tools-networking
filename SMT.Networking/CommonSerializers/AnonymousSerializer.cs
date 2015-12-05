using SMT.Networking.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT.Networking
{
    public class AnonymousSerializer<T> : INetworkConnectionSerializer<T>
    {
        private readonly Func<byte[], T> Deserializer;
        private readonly Func<T, byte[]> Serializer;

        public AnonymousSerializer(Func<byte[], T> deserialize, Func<T, byte[]> serialize)
        {
            this.Deserializer = deserialize;
            this.Serializer = serialize;
        }

        public byte[] Serialize(T message)
        {
            return Serializer(message);
        }

        public T Deserialize(byte[] data)
        {
            return Deserializer(data);
        }
    }
}
