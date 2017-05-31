using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServiceFabric.Utils.Http.Logging
{
    /// <summary>
    /// Provides a way to filter headers from an <see cref="IHeaderDictionary"/>.
    /// </summary>
    public class HeaderFilter : IHeaderFilter
    {
        Func<KeyValuePair<string, string[]>, bool> _filter;

        /// <summary>
        /// Creates a new instance of <see cref="HeaderFilter"/> with the specified <paramref name="filter"/>.
        /// </summary>
        /// <param name="filter"></param>
        private HeaderFilter(Func<KeyValuePair<string, string[]>, bool> filter)
        {
            _filter = filter;
        }

        /// <summary>
        /// Gets a filtered <see cref="IHeaderDictionary"/>.
        /// </summary>
        /// <param name="headers">The <see cref="IHeaderDictionary"/> to filter</param>
        /// <returns>An new instance of <see cref="IHeaderDictionary"/></returns>
        public IHeaderDictionary GetHeaders(IHeaderDictionary headers)
        {
            return new HeaderDictionary(headers.Where(_filter).ToDictionary(x => x.Key, x => x.Value));
        }

        /// <summary>
        /// Creates a new instance of <see cref="HeaderFilter"/> which excludes the 'authorization' header .
        /// </summary>
        /// <returns>A new instance of <see cref="HeaderFilter"/></returns>
        public static HeaderFilter CreateExcludeFilterAuthorization()
        {
            return CreateExcludeFilter("authorization");
        }

        /// <summary>
        /// Creates a new instance of <see cref="HeaderFilter"/> which excludes the specified header(s).
        /// </summary>
        /// <param name="headersToExclude">A collection of names of the headers to exclude</param>
        /// <returns>A new instance of <see cref="HeaderFilter"/></returns>
        public static HeaderFilter CreateExcludeFilter(IEnumerable<string> headersToExclude)
        {
            return new HeaderFilter(x => !headersToExclude.Any(y => string.Equals(x.Key, y, StringComparison.InvariantCultureIgnoreCase)));
        }

        /// <summary>
        /// Creates a new instance of <see cref="HeaderFilter"/> which excludes the specified header(s).
        /// </summary>
        /// <param name="headersToExclude">A collection of names of the headers to exclude</param>
        /// <returns>A new instance of <see cref="HeaderFilter"/></returns>
        public static HeaderFilter CreateExcludeFilter(params string[] headersToExclude)
        {
            return new HeaderFilter(x => !headersToExclude.Any(y => string.Equals(x.Key, y, StringComparison.InvariantCultureIgnoreCase)));
        }

        /// <summary>
        /// Creates a new instance of <see cref="HeaderFilter"/> which includes the specified header(s).
        /// </summary>
        /// <param name="headersToInclude">A collection of names of the headers to include</param>
        /// <returns>A new instance of <see cref="HeaderFilter"/></returns>
        public static HeaderFilter CreateIncludeFilter(IEnumerable<string> headersToInclude)
        {
            return new HeaderFilter(x => headersToInclude.Any(y => string.Equals(x.Key, y, StringComparison.InvariantCultureIgnoreCase)));
        }

        /// <summary>
        /// Creates a new instance of <see cref="HeaderFilter"/> which includes the specified header(s).
        /// </summary>
        /// <param name="headersToInclude">A collection of names of the headers to include</param>
        /// <returns>A new instance of <see cref="HeaderFilter"/></returns>
        public static HeaderFilter CreateIncludeFilter(params string[] headersToInclude)
        {
            return new HeaderFilter(x => headersToInclude.Any(y => string.Equals(x.Key, y, StringComparison.InvariantCultureIgnoreCase)));
        }
    }
}
