using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin;

namespace ServiceFabric.Utils.Logging
{
    public interface IErrorHandler
    {
        Task<Guid> LogErrorAsync(Error error);
        Task<Guid> LogErrorAsync(IOwinContext context, HttpStatusCode statusCode, Exception exception);
        Task<Guid> LogErrorAsync(IOwinContext context, HttpStatusCode statusCode, HttpError httpError);
        Task<Guid> LogErrorAsync(string applicationName, string applicationVersion, IOwinContext context, HttpStatusCode statusCode, Exception exception);
        Task<Guid> LogErrorAsync(string applicationName, string applicationVersion, IOwinContext context, HttpStatusCode statusCode, HttpError httpError);
    }
}