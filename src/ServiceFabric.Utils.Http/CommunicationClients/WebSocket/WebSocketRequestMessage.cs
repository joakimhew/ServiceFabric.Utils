using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceFabric.Utils.Http.CommunicationClients.WebSocket
{
    [ProtoContract]
    public class WebSocketRequestMessage
    {
        [ProtoMember(1)] public int PartitionKey;
        [ProtoMember(2)] public string Operation;
        [ProtoMember(3)] public byte[] Value;
    }
}
