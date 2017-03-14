using System.Net;
using Newtonsoft.Json;

namespace ServiceFabric.Utils.Ipc.Http
{
    public class ApiResponseMessage<TMessage>
    {
        [JsonProperty("code")]
        public HttpStatusCode Code { get; set; }

        [JsonProperty("message")]
        public TMessage Message { get; set; }

        [JsonProperty("info")]
        public string Info { get; set; }
    }
}