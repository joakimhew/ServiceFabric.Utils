using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceFabric.Utils.Http.CommunicationClients.WebSocket
{
    public class ProtobufWebSocketSerializer : IWebSocketSerializer
    {
        public Task<T> DeserializeAsync<T>(byte[] serialized)
        {
            return DeserializeAsync<T>(serialized, 0, serialized.Length);
        }

        public Task<T> DeserializeAsync<T>(byte[] serialized, int offset, int length)
        {
            using (MemoryStream ms = new MemoryStream(serialized, offset, length))
            {
                return Task.FromResult(Serializer.Deserialize<T>(ms));
            }
        }

        public Task<byte[]> SerializeAsync<T>(T value)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Serializer.Serialize(ms, value);
                return Task.FromResult(ms.ToArray());
            }
        }
    }
}
