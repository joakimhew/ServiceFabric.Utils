using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;

namespace ServiceFabric.Utils.Ipc.Http
{
    public class ApiHttpExceptionHandler : ExceptionHandler
    {
        private readonly IErrorHandler _errorHandler;

        public ApiHttpExceptionHandler(IErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
        }

        public override async Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            var owinContext = context.Request.GetOwinContext();
            var errorId = await _errorHandler.LogErrorAsync(owinContext, HttpStatusCode.InternalServerError, context.Exception);

            context.Result = new ApiHttpActionResult(
                context.Request,
                HttpStatusCode.InternalServerError,
                "Internal server error",
                errorId == Guid.Empty ? null : errorId.ToString());
        }

        public override bool ShouldHandle(ExceptionHandlerContext context)
        {
            return true;
        }
    }
}