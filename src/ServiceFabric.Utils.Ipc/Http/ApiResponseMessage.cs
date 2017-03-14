using System.Net;
using Newtonsoft.Json;

namespace ServiceFabric.Utils.Ipc.Http
{
    public class ApiResponseMessage
    {
        [JsonProperty("code")]
        public HttpStatusCode Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("info")]
        public string Info { get; set; }
    }
}