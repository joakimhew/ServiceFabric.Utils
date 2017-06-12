using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin;

namespace ServiceFabric.Utils.Logging
{
    /// <summary>
    /// Used to log an instance of <see cref="Error"/>, <see cref="HttpError"/> or <see cref="Exception"/> with the provided implementation of <see cref="IErrorStore"/>
    /// </summary>
    public class ErrorHandler : IErrorHandler
    {
        private readonly IErrorStore _errorStore;
        private readonly string _applicationName;
        private readonly string _applicationVersion;

        /// <summary>
        /// Creates a new instance of <see cref="ErrorHandler"/>
        /// </summary>
        /// <param name="errorStore"><see cref="IErrorStore"/> used to wirte the instance of <see cref="Error"/> to something like a database or file</param>
        /// <param name="assembly">The <see cref="Assembly"/> that is logging the error</param>
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

        /// <summary>
        /// Creates a new instance of <see cref="ErrorHandler"/>
        /// </summary>
        /// <param name="errorStore"><see cref="IErrorStore"/> used to wirte the instance of <see cref="Error"/> to something like a database or file</param>
        /// <param name="applicationName">The name of the application that is logging the error</param>
        /// <param name="applicationVersion">The version of the application that is logging the error</param>
        public ErrorHandler(IErrorStore errorStore, string applicationName, string applicationVersion)
        {
            _errorStore = errorStore;
            _applicationName = applicationName;
            _applicationVersion = applicationVersion;
        }

        /// <summary>
        /// Logs an instance of <see cref="Exception"/>
        /// </summary>
        /// <param name="context">The <see cref="IOwinContext"/> that caused the error</param>
        /// <param name="statusCode">The <see cref="HttpStatusCode"/> that is associated with the error</param>
        /// <param name="exception">The <see cref="Exception"/> that is associated with the error</param>
        /// <returns>A new <see cref="Guid"/> to uniquely identify the error</returns>
        public async Task<Guid> LogErrorAsync(IOwinContext context, HttpStatusCode statusCode, Exception exception)
        {
            return await LogErrorAsync(_applicationName, _applicationVersion, context, statusCode, exception);
        }

        /// <summary>
        /// Logs an instance of <see cref="Exception"/>
        /// </summary>
        /// <param name="context">The <see cref="IOwinContext"/> that caused the error</param>
        /// <param name="statusCode">The <see cref="HttpStatusCode"/> that is associated with the error</param>
        /// <param name="httpError">The <see cref="HttpError"/> that is associated with the error</param>
        /// <returns>A new <see cref="Guid"/> to uniquely identify the error</returns>
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
            var error = new Error(context)
                .WithApplicationName(applicationName)
                .WithApplicationVersion(applicationVersion)
                .WithMachineName()
                .WithHost()
                .WithUrl()
                .WithHttpMethod()
                .WithHttpStatusCode((int) statusCode)
                .WithIpAddress()
                .WithQueryString()
                .WithForm()
                .WithCookies()
                .WithRequestHeaders()
                .WithType(Enum.GetName(typeof(HttpStatusCode), statusCode))
                .WithMessage(
                    string.IsNullOrEmpty(httpError.Message) 
                    ? "Failed to parse httpError.Message" 
                    : httpError.Message)
                .WithDetail(
                    string.IsNullOrEmpty(httpError.MessageDetail) 
                    ? $"Failed to parse httpError.MessageDetail. Inner exception: {httpError.InnerException}" 
                    : httpError.MessageDetail)
                .WithFullStackTrace();

            var result = await _errorStore.AddAsync(error);

            return result == 1 ? error.Id : Guid.Empty;
        }
    }
}