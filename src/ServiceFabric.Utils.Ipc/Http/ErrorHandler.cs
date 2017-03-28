using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin;

namespace ServiceFabric.Utils.Ipc.Http
{
    public class ErrorHandler : IErrorHandler
    {
        private readonly IErrorStore _errorStore;
        private readonly string _applicationName;
        private readonly string _applicationVersion;

        public ErrorHandler(IErrorStore errorStore, Assembly assembly)
        {
            _errorStore = errorStore;
            _applicationName = assembly.GetName().Name;

            if (assembly.Location == null)
            {
                return;
            }

            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            _applicationVersion = fileVersionInfo.FileVersion;
        }

        public ErrorHandler(IErrorStore errorStore, string applicationName, string applicationVersion)
        {
            _errorStore = errorStore;
            _applicationName = applicationName;
            _applicationVersion = applicationVersion;
        }

        public async Task<Guid> LogErrorAsync(IOwinContext context, HttpStatusCode statusCode, Exception exception)
        {
            return await LogErrorAsync(_applicationName, _applicationVersion, context, statusCode, exception);
        }

        public async Task<Guid> LogErrorAsync(IOwinContext context, HttpStatusCode statusCode, HttpError httpError)
        {
            return await LogErrorAsync(_applicationName, _applicationVersion, context, statusCode, httpError);
        }

        public async Task<Guid> LogErrorAsync(Error error)
        {
            var result = await _errorStore.AddAsync(error);

            return result == 1 ? error.Id : Guid.Empty;
        }

        public async Task<Guid> LogErrorAsync(
            string applicationName,
            string applicationVersion,
            IOwinContext context,
            HttpStatusCode statusCode,
            Exception exception)
        {
            var error = new Error(exception, context)
                .WithApplicationName(applicationName)
                .WithApplicationVersion(applicationVersion)
                .WithMachineName()
                .WithHost()
                .WithUrl()
                .WithHttpMethod()
                .WithHttpStatusCode((int)statusCode)
                .WithIpAddress()
                .WithQueryString()
                .WithForm()
                .WithCookies()
                .WithRequestHeaders()
                .WithAllExceptionProperties();

            var result = await _errorStore.AddAsync(error);

            return result == 1 ? error.Id : Guid.Empty;
        }

        public async Task<Guid> LogErrorAsync(
            string applicationName,
            string applicationVersion,
            IOwinContext context,
            HttpStatusCode statusCode,
            HttpError httpError)
        {
            var exception = new ApiException(httpError.MessageDetail, httpError.StackTrace);

            var error = new Error(exception, context)
                .WithApplicationName(applicationName)
                .WithApplicationVersion(applicationVersion)
                .WithMachineName()
                .WithHost()
                .WithUrl()
                .WithHttpMethod()
                .WithHttpStatusCode((int)statusCode)
                .WithIpAddress()
                .WithQueryString()
                .WithForm()
                .WithCookies()
                .WithRequestHeaders()
                .WithType(httpError.ExceptionType)
                .WithMessage(exception.Message)
                .WithDetail(exception.StackTrace)
                .WithSource()
                .WithFullStackTrace();

            var result = await _errorStore.AddAsync(error);

            return result == 1 ? error.Id : Guid.Empty;
        }
    }
}