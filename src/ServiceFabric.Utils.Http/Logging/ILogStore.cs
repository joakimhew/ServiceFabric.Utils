using System;
using System.Threading.Tasks;

namespace ServiceFabric.Utils.Http.Logging
{
    /// <summary>
    /// Logs a <see cref="Request"/> or <see cref="Response"/>.
    /// </summary>
    public interface ILogStore : IDisposable
    {
        /// <summary>
        /// Add a <see cref="Request"/>.
        /// </summary>
        /// <param name="request">The <see cref="Request"/>.</param>
        /// <returns>True on success; otherwise false</returns>
        Task<bool> AddAsync(Request request);
        /// <summary>
        /// Add a <see cref="Response"/>.
        /// </summary>
        /// <param name="response">The <see cref="Response"/>.</param>
        /// <returns>True on success; otherwise false</returns>
        Task<bool> AddAsync(Response response);
    }
}
