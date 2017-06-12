using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceFabric.Utils.CommunicationClients.WebSocket
{
    [ProtoContract]
    public class WebSocketResponseMessage
    {
        [ProtoMember(1)] public int Result;
        [ProtoMember(2)] public byte[] Value;
    }
}
