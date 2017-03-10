using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Newtonsoft.Json;

namespace ServiceFabric.Utils.Ipc.Http
{
    public abstract class HttpApiResponseMessage : IHttpActionResult
    {
        private readonly HttpRequestMessage _requestMessage;
        private readonly HttpStatusCode _statusCode;
        private readonly object _message;

        protected HttpApiResponseMessage(HttpRequestMessage request, HttpStatusCode statusCode, object message)
        {
            _requestMessage = request;
            _statusCode = statusCode;
            _message = message;
        }

        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var body = new
            {
                code = _statusCode,
                message = _message
            };

            var formattedContentResult =
                new FormattedContentResult<object>(
                    _statusCode,
                    body,
                    new JsonMediaTypeFormatter(),
                    new MediaTypeHeaderValue("application/json"),
                    _requestMessage);

            HttpResponseMessage response = await formattedContentResult.ExecuteAsync(cancellationToken);

            return response;
        }
    }
}