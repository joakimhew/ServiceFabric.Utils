using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace ServiceFabric.Utils.Ipc.Http
{
    public class HttpApiResponseMessage : IHttpActionResult
    {
        private readonly HttpRequestMessage _requestMessage;
        private readonly HttpStatusCode _code;
        private readonly object _message;
        private readonly object _info;

        public HttpApiResponseMessage(HttpRequestMessage request, HttpStatusCode statusCode,
            object message, object additionalInfo = null)
        {
            _requestMessage = request;
            _code = statusCode;
            _message = message;
            _info = additionalInfo;
        }

        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var body = new
            {
                code = _code,
                message = _message,
                info = _info
            };

            var formattedContentResult =
                new FormattedContentResult<object>(
                    _code,
                    body,
                    new JsonMediaTypeFormatter(),
                    new MediaTypeHeaderValue("application/json"),
                    _requestMessage);

            HttpResponseMessage response = await formattedContentResult.ExecuteAsync(cancellationToken);

            return response;
        }
    }
}
