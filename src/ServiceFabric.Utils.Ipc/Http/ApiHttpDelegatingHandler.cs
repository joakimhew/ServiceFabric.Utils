using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
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
            if (response.StatusCode == HttpStatusCode.InternalServerError)
                return response;

            HttpError httpError;

            if (!response.TryGetContentValue(out httpError))
                return response;

            DebugHttpErrors("HTTPERROR MAIN", httpError);
            DebugHttpErrors("HTTPERROR INNER", httpError.InnerException);
            DebugHttpErrors("HTTPERROR MODELSTATE", httpError.ModelState);

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

        private void DebugHttpErrors(string header, HttpError httpError)
        {
            Debug.WriteLine($"{Environment.NewLine}------ {header} start -------");

            if (httpError == null)
            {
                Debug.WriteLine($"No HttpError for {header}");
            }
            else
            {
                WriteAllProperties();
                WriteAllKeys();
            }

            Debug.WriteLine($"------ end {header} end -------{Environment.NewLine}");

            void WriteAllProperties()
            {
                Debug.WriteLine($"Message: {httpError.Message}");
                Debug.WriteLine($"Message details: {httpError.MessageDetail}");
                Debug.WriteLine($"Exception message: {httpError.ExceptionMessage}");
                Debug.WriteLine($"Exception type: {httpError.ExceptionType}");
                Debug.WriteLine($"StackTrace: {httpError.StackTrace}");
                Debug.WriteLine($"{Environment.NewLine}--- {header} KEYS ---");
            }

            void WriteAllKeys() => httpError.ToList().ForEach(x => { Debug.WriteLine($"KEY: {x.Key} VALUE: {x.Value}"); });
        }

        public event Func<IOwinContext, HttpResponseMessage, HttpResponseMessage> ResponseMessageHandler;
    }
    
    
}