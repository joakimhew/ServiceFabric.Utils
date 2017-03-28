using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ServiceFabric.Utils.Ipc.Http
{
    public static class HttpRequestMessageExtensions
    {
        public static HttpResponseMessage CreateApiResponse(this HttpRequestMessage request,
            HttpStatusCode statusCode, object message, object additionalInfo = null)
        {
            return new ApiHttpResponseMessage(statusCode, message, additionalInfo);
        }
    }
}