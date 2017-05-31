using Microsoft.Owin;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace ServiceFabric.Utils.Http.Logging
{
    /// <summary>
    /// Provides a way to log request and response.
    /// </summary>
    public class LogHandler : ILogHandler
    {
        private readonly ILogStore _logStore;
        private readonly IClaimResolver _claimResolver;
        private readonly IHeaderFilter _requestHeaderFilter;

        private readonly string _applicationName;
        private readonly string _applicationVersion;

        /// <summary>
        /// Creates a new instance of <see cref="LogHandler"/> with the specified parameters.
        /// </summary>
        /// <param name="logStore">The <see cref="ILogStore"/></param>
        /// <param name="claimResolver">the <see cref="IClaimResolver"/></param>
        /// <param name="requestHeaderFilter">The <see cref="IHeaderFilter"/> for the request headers</param>
        /// <param name="assembly">The calling assembly</param>
        /// <exception cref="ArgumentNullException"/>
        public LogHandler(ILogStore logStore, IClaimResolver claimResolver, IHeaderFilter requestHeaderFilter, Assembly assembly)
        {
            _logStore = logStore ?? throw new ArgumentNullException(nameof(logStore));
            _claimResolver = claimResolver ?? throw new ArgumentNullException(nameof(claimResolver));
            _requestHeaderFilter = requestHeaderFilter ?? throw new ArgumentNullException(nameof(requestHeaderFilter));

            _applicationName = assembly.GetName().Name;
            _applicationVersion = FileVersionInfo.GetVersionInfo(assembly.Location)?.FileVersion;

        }
        /// <summary>
        /// Logs the specified <see cref="Request"/>.
        /// </summary>
        /// <param name="request">The <see cref="Request"/></param>
        /// <returns>
        /// The <see cref="Guid"/> assigned to the <see cref="Request"/>.
        /// Will return <see cref="Guid.Empty"/> if failed to log the <see cref="Request"/>.
        /// </returns>
        public async Task<Guid> LogRequestAsync(Request request)
        {
            var success = await _logStore.AddAsync(request);

            return success ? request.Id : Guid.Empty;
        }
        /// <summary>
        /// Logs the request from the specified <see cref="IOwinContext"/>.
        /// </summary>
        /// <param name="context">The <see cref="IOwinContext"/></param>
        /// <returns>
        /// The <see cref="Guid"/> assigned to the <see cref="Request"/>.
        /// Will return <see cref="Guid.Empty"/> if failed to log the <see cref="Request"/>.
        /// </returns>
        public async Task<Guid> LogRequestAsync(IOwinContext context)
        {
            var request = new Request(context, _claimResolver);

            request.WithApplicationName(_applicationName )
                .WithApplicationVersion(_applicationVersion)
                .WithClaims()
                .WithUsername()
                .WithCookies()
                .WithForm()
                .WithHttpMethod()
                .WithIpAddress()
                .WithMachineName()
                .WithQueryString()
                .WithRequestHeaders(_requestHeaderFilter.GetHeaders(context.Request.Headers))
                .WithUrl();

            return await LogRequestAsync(request);
        }
        /// <summary>
        /// Logs the specified <see cref="Response"/>.
        /// </summary>
        /// <param name="response">The <see cref="Response"/></param>
        /// <returns>
        /// The <see cref="Guid"/> assigned to the <see cref="Response"/>.
        /// Will return <see cref="Guid.Empty"/> if failed to log the <see cref="Response"/>.
        /// </returns>
        public async Task<Guid> LogResponseAsync(Response response)
        {
            var success = await _logStore.AddAsync(response);

            return success ? response.Id : Guid.Empty;
        }
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
        public async Task<Guid> LogResponseAsync(IOwinContext context, Guid? requestId, long elapsedMilliseconds)
        {
            var response = requestId.GetValueOrDefault(Guid.Empty) == Guid.Empty ? new Response(context) : new Response(context, requestId.Value);

            response.ElapsedMilliseconds = elapsedMilliseconds;

            response.WithApplicationName(_applicationName)
                .WithApplicationVersion(_applicationVersion)
                .WithHttpStatusCode()
                .WithMachineName()
                .WithResponseHeaders();

            return await LogResponseAsync(response);
        }
    }
}
