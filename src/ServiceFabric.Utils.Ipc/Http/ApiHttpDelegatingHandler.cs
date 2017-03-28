using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin;

namespace ServiceFabric.Utils.Ipc.Http
{
    public class ApiHttpDelegatingHandler : DelegatingHandler
    {
        private readonly IErrorHandler _errorHandler;

        public ApiHttpDelegatingHandler(IErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
           CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);
            var context = request.GetOwinContext();

            return ResponseMessageHandler != null
                ? ResponseMessageHandler.Invoke(context, response)
                : await DefaultResponseMessageHandler(context, response);
        }

        private async Task<HttpResponseMessage> DefaultResponseMessageHandler(IOwinContext context, HttpResponseMessage response)
        {
            if (!response.TryGetContentValue(out HttpError httpError))
                return response;

            Guid errorId;

            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    errorId = await _errorHandler.LogErrorAsync(context, response.StatusCode, httpError);
                    return new ApiHttpResponseMessage(
                        response.StatusCode,
                        "bad request",
                        errorId == Guid.Empty ? null : errorId.ToString());

                default:
                    errorId = await _errorHandler.LogErrorAsync(context, response.StatusCode, httpError);
                    return new ApiHttpResponseMessage(
                        response.StatusCode,
                        "an error has occured",
                        errorId == Guid.Empty ? null : errorId.ToString());
            }
        }

        public event Func<IOwinContext, HttpResponseMessage, HttpResponseMessage> ResponseMessageHandler;
    }
}