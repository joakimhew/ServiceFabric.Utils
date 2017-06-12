using Microsoft.Owin;

namespace ServiceFabric.Utils.Logging
{
    /// <summary>
    /// Provides a way to filter headers from an <see cref="IHeaderDictionary"/>.
    /// </summary>
    public interface IHeaderFilter
    {
        /// <summary>
        /// Gets a filtered <see cref="IHeaderDictionary"/>.
        /// </summary>
        /// <param name="headers">The <see cref="IHeaderDictionary"/> to filter</param>
        /// <returns>An new instance of <see cref="IHeaderDictionary"/></returns>
        IHeaderDictionary GetHeaders(IHeaderDictionary headers);
    }
}
