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
            HttpError httpError;

            if (!response.TryGetContentValue(out httpError))
                return response;

            Guid errorId;

            switch (response.StatusCode)
            {
                default:
                    errorId = await _errorHandler.LogErrorAsync(context, response.StatusCode, httpError);
                    return new ApiHttpResponseMessage(
                        response.StatusCode,
                        httpError.Message,
                        errorId == Guid.Empty ? null : errorId.ToString());
            }
        }

        public event Func<IOwinContext, HttpResponseMessage, HttpResponseMessage> ResponseMessageHandler;
    }
}