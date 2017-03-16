using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Reflection;
using System.Web.Http.ExceptionHandling;
using Microsoft.ServiceFabric.Services.Communication.Client;
using IExceptionHandler = Microsoft.ServiceFabric.Services.Communication.Client.IExceptionHandler;

namespace ServiceFabric.Utils.Ipc.Http
{
    public class HttpExceptionHandler : ExceptionHandler, IExceptionHandler
    {
        private readonly IErrorStore _errorStore;

        public HttpExceptionHandler()
        {
        }

        public HttpExceptionHandler(IErrorStore errorStore)
        {
            _errorStore = errorStore;
        }

        public bool TryHandleException(ExceptionInformation exceptionInformation, OperationRetrySettings retrySettings,
            out ExceptionHandlingResult result)
        {
            if (exceptionInformation.Exception is TimeoutException)
            {
                result = 
                    new ExceptionHandlingRetryResult(
                        exceptionInformation.Exception, 
                        false, 
                        retrySettings, 
                        retrySettings.DefaultMaxRetryCount);

                return true;
            }

            if (exceptionInformation.Exception is SocketException)
            {
                result =
                    new ExceptionHandlingRetryResult(
                        exceptionInformation.Exception,
                        false,
                        retrySettings,
                        retrySettings.DefaultMaxRetryCount);

                return true;
            }

            if (exceptionInformation.Exception is ProtocolViolationException)
            {
                result = new ExceptionHandlingThrowResult();
                return true;
            }

            var we = exceptionInformation.Exception.InnerException as WebException ??
                              exceptionInformation.Exception.InnerException as WebException;

            if (we != null)
            {
                var errorResponse = we.Response as HttpWebResponse;

                if (we.Status == WebExceptionStatus.ProtocolError)
                {
                    if (errorResponse != null && errorResponse.StatusCode == HttpStatusCode.NotFound)
                    {
                        result = 
                            new ExceptionHandlingRetryResult(
                                exceptionInformation.Exception,
                                false,
                                retrySettings,
                                retrySettings.DefaultMaxRetryCount);

                        return true;
                    }

                    if (errorResponse != null && errorResponse.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        result = 
                            new ExceptionHandlingRetryResult(
                                exceptionInformation.Exception,
                                true,
                                retrySettings,
                                retrySettings.DefaultMaxRetryCount);

                        return true;
                    }
                }

                if (we.Status == WebExceptionStatus.Timeout ||
                    we.Status == WebExceptionStatus.RequestCanceled ||
                    we.Status == WebExceptionStatus.ConnectionClosed ||
                    we.Status == WebExceptionStatus.ConnectFailure)
                {
                    result = 
                        new ExceptionHandlingRetryResult(
                            exceptionInformation.Exception,
                            false,
                            retrySettings,
                            retrySettings.DefaultMaxRetryCount);

                    return true;
                }
            }

            result = null;
            return false;
        }

        public override void Handle(ExceptionHandlerContext context)
        {
            var error = new Error(context.Exception, context.Request.GetOwinContext())
                .WithApplicationName(Assembly.GetCallingAssembly().GetName().Name)
                .WithMachineName()
                .WithAllContextProperties()
                .WithAllExceptionProperties();

            _errorStore.AddAsync(error);

            context.Result = new HttpApiResponseMessage(
                context.Request,
                HttpStatusCode.InternalServerError,
                "And error has occured. Please contact the administrators",
                error.Id);
        }
    }
}