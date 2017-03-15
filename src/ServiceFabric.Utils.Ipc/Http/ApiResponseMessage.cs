using System.Net;
using Newtonsoft.Json;

namespace ServiceFabric.Utils.Ipc.Http
{
    public class ApiResponseMessage<TMessageType>
    {
        [JsonProperty("code")]
        public HttpStatusCode Code { get; set; }

        [JsonProperty("message")]
        public TMessageType Message { get; set; }

        [JsonProperty("info")]
        public string Info { get; set; }
    }
}