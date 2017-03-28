using System;
using System.Diagnostics;
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
        private readonly string _applicationName;
        private readonly string _applicationVersion;
        private readonly IErrorStore _errorStore;

        public HttpExceptionHandler()
        {
        }

        public HttpExceptionHandler(IErrorStore errorStore, Assembly callingAssembly)
        {
            _errorStore = errorStore;
            _applicationName = callingAssembly.GetName().Name;

            if (callingAssembly.Location == null)
            {
                return;
            }

            var fileVersionInfo = FileVersionInfo.GetVersionInfo(callingAssembly.Location);
            _applicationVersion = fileVersionInfo.FileVersion;
        }

        public HttpExceptionHandler(IErrorStore errorStore, string applicationName = null, string applicationVersion = null)
        {
            _errorStore = errorStore;
            _applicationName = applicationName;
            _applicationVersion = applicationVersion;
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
    }
}