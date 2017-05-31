using Microsoft.Owin;
using System;
using System.Threading.Tasks;

namespace ServiceFabric.Utils.Http.Logging
{
    /// <summary>
    /// Provides a way to log request and response.
    /// </summary>
    public interface ILogHandler
    {
        /// <summary>
        /// Logs the specified <see cref="Request"/>.
        /// </summary>
        /// <param name="request">The <see cref="Request"/></param>
        /// <returns>
        /// The <see cref="Guid"/> assigned to the <see cref="Request"/>.
        /// Will return <see cref="Guid.Empty"/> if failed to log the <see cref="Request"/>.
        /// </returns>
        Task<Guid> LogRequestAsync(Request request);
        /// <summary>
        /// Logs the request from the specified <see cref="IOwinContext"/>.
        /// </summary>
        /// <param name="context">The <see cref="IOwinContext"/></param>
        /// <returns>
        /// The <see cref="Guid"/> assigned to the <see cref="Request"/>.
        /// Will return <see cref="Guid.Empty"/> if failed to log the <see cref="Request"/>.
        /// </returns>
        Task<Guid> LogRequestAsync(IOwinContext context);

        /// <summary>
        /// Logs the specified <see cref="Response"/>.
        /// </summary>
        /// <param name="response">The <see cref="Response"/></param>
        /// <returns>
        /// The <see cref="Guid"/> assigned to the <see cref="Response"/>.
        /// Will return <see cref="Guid.Empty"/> if failed to log the <see cref="Response"/>.
        /// </returns>
        Task<Guid> LogResponseAsync(Response response);
        /// <summary>
        /// Logs the response from the specified <see cref="IOwinContext"/>.
        /// </summary>
        /// <param name="context">The <see cref="IOwinContext"/></param>
        /// <param name="requestId">The <see cref="Guid"/> associated with this response's request (Optional).</param>
        /// <param name="elapsedMilliseconds">The elapsed time in milliseconds since the request was made.</param>
        /// <returns>
        /// The <see cref="Guid"/> assigned to the <see cref="Response"/>.
        /// Will return <see cref="Guid.Empty"/> if failed to log the <see cref="Response"/>.
        /// </returns>
        Task<Guid> LogResponseAsync(IOwinContext context, Guid? requestId, long elapsedMilliseconds);
    }
}
