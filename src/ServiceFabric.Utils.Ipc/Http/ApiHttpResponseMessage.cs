using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ServiceFabric.Utils.Ipc.Http
{
    public class ApiHttpResponseMessage : HttpResponseMessage
    {
        public ApiHttpResponseMessage(HttpStatusCode statusCode, object message, object additionalInfo = null)
        {
            var body = new
            {
                code = statusCode,
                message,
                info = additionalInfo
            };

            base.Content = new ObjectContent<object>(body, new JsonMediaTypeFormatter
            {
                SerializerSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }
            });

            base.StatusCode = statusCode;
        }
    }
}