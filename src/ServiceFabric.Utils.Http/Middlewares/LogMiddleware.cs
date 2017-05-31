using Microsoft.Owin;
using ServiceFabric.Utils.Http.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ServiceFabric.Utils.Http.Middlewares
{
    /// <summary>
    /// Logs <see cref="IOwinRequest"/> and <see cref="IOwinResponse"/>.
    /// </summary>
    public class LogMiddleware : OwinMiddleware
    {
        private OwinMiddleware _next;
        private ILogHandler _logHandler;

        /// <summary>
        /// Creates a new instance of <see cref="LogMiddleware"/> with the specified <see cref="ILogHandler"/>.
        /// </summary>
        /// <param name="next">The next <see cref="OwinMiddleware"/> in the chain</param>
        /// <param name="logHandler">The <see cref="ILogHandler"/>.</param>
        /// <exception cref="ArgumentNullException"
        public LogMiddleware(OwinMiddleware next, ILogHandler logHandler) : base(next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logHandler = logHandler ?? throw new ArgumentNullException(nameof(logHandler));
        }

        /// <summary>
        /// Process an individual request and response.
        /// </summary>
        /// <param name="context">The <see cref="IOwinContext"/></param>
        /// <returns><see cref="Task"/></returns>
        public override async Task Invoke(IOwinContext context)
        {
            var requestId = await _logHandler.LogRequestAsync(context);

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            await _next.Invoke(context);

            var elapsedMilliseconds = stopWatch.ElapsedMilliseconds;
            stopWatch.Stop();

            await _logHandler.LogResponseAsync(context, requestId, elapsedMilliseconds);
        }
    }
}
