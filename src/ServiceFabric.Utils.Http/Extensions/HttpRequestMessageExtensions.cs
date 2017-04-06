using System.Net;
using System.Net.Http;

namespace ServiceFabric.Utils.Http.Extensions
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