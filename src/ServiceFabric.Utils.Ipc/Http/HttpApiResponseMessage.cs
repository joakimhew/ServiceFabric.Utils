using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace ServiceFabric.Utils.Ipc.Http
{
    public abstract class HttpApiResponseMessage : HttpResponseMessage, IApiResponseMessage<HttpStatusCode>
    {
        protected HttpApiResponseMessage(HttpStatusCode statusCode, object message, object additionalInfo)
            : base(statusCode)
        {
            Code = statusCode;
            Message = message;
            AdditionalInfo = additionalInfo;

            base.Content =
                new StringContent(JsonConvert.SerializeObject(new
                {
                    code = Code,
                    message = Message,
                    additional_info = AdditionalInfo
                }));

            base.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/json");
        }

        public HttpStatusCode Code { get; }

        public object Message { get; }

        public object AdditionalInfo { get; }
    }
}