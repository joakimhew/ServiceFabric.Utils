using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ServiceFabric.Utils.Ipc.Http
{
    public static class HttpRequestApiMessageExtensions
    {
        public static HttpResponseMessage CreateApiResponse(this HttpRequestMessage request,
            HttpStatusCode statusCode, object message, object additionalInfo = null)
        {
            var body = new
            {
                code = statusCode,
                message,
                info = additionalInfo
            };

            return request.CreateResponse(statusCode, body, new JsonMediaTypeFormatter
            {
                SerializerSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }
            });
        }
    }
}