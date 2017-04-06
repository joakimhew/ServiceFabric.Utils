using System.Net;

namespace ServiceFabric.Utils.Http
{
    public class ApiResponseMessage<TMessageType>
    {
        public HttpStatusCode Code { get; set; }

        public TMessageType Message { get; set; }

        public string Info { get; set; }
    }
}