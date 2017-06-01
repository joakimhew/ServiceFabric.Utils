using Microsoft.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace ServiceFabric.Utils.Http.Logging
{
    /// <summary>
    /// Log data of a request
    /// </summary>
    public class Request
    {
        private readonly IOwinContext _context;
        private readonly IClaimResolver _claimResolver;

        /// <summary>
        /// Creates a new instance of <see cref="Request"/> with the specified <see cref="IOwinContext"/>.
        /// </summary>
        /// <param name="context">The <see cref="<see cref="IOwinContext"/></param>
        public Request(IOwinContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            this.Id = Guid.NewGuid();
            this.Created = DateTime.UtcNow;
        }

        /// <summary>
        /// Creates a new instance of <see cref="Request"/> with the specified <see cref="IOwinContext"/> and <see cref="IClaimResolver"/>.
        /// </summary>
        /// <param name="context">The <see cref="<see cref="IOwinContext"/>"/></param>
        /// <param name="claimResolver">The <see cref="IClaimResolver"/></param>
        public Request(IOwinContext context, IClaimResolver claimResolver)
            : this(context)
        {
            _claimResolver = claimResolver ?? throw new ArgumentNullException(nameof(claimResolver));
        }

        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Gets or sets the Application Name.
        /// </summary>
        public string ApplicationName { get; set; }
        /// <summary>
        /// Gets or sets the Application Version.
        /// </summary>
        public string ApplicationVersion { get; set; }
        /// <summary>
        /// Gets or sets the Machine Name.
        /// </summary>
        public string MachineName { get; set; }
        /// <summary>
        /// Gets or sets the Created date.
        /// </summary>
        public DateTime Created { get; set; }
        /// <summary>
        /// Gets or sets the Url.
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Gets or sets the Http Method.
        /// </summary>
        public string HttpMethod { get; set; }
        /// <summary>
        /// Gets or sets the Ip Address.
        /// </summary>
        public string IpAddress { get; set; }
        /// <summary>
        /// Gets or sets the Query String.
        /// </summary>
        public QueryString QueryString { get; set; }
        /// <summary>
        /// Gets or sets the Form Collection.
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// Gets or sets the Request Cookies.
        /// </summary>
        public RequestCookieCollection Cookies { get; set; }
        /// <summary>
        /// Gets or sets the Request Headers.
        /// </summary>
        public IHeaderDictionary RequestHeaders { get; set; }
        /// <summary>
        /// Gets or sets the Claims.
        /// </summary>
        public List<Claim> Claims { get; set; }
        /// <summary>
        /// Gets or sets the Username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets the current instance serialized as a Json string.
        /// </summary>
        [JsonIgnore]
        public string FullJson
        {
            get
            {
                var json = JsonConvert.SerializeObject(this, Formatting.None, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });

                return json;
            }
        }

        /// <summary>
        /// Include application name with the request.
        /// </summary>
        /// <param name="applicationName">Optional custom application name. Defaults to calling assembly name</param>
        /// <returns>The current <see cref="Request"/> instance with the application name</returns>
        public Request WithApplicationName(string applicationName = null)
        {
            if (string.IsNullOrEmpty(applicationName))
            {
                ApplicationName = Assembly.GetCallingAssembly().FullName;
                return this;
            }

            ApplicationName = applicationName;
            return this;
        }

        /// <summary>
        /// Include application version with the request.
        /// </summary>
        /// <param name="applicationVersion">Optional custom application version. Defaults to FileVersion of assembly</param>
        /// <returns>The current <see cref="Request"/> instance with the application version</returns>
        public Request WithApplicationVersion(string applicationVersion = null)
        {
            if (string.IsNullOrEmpty(applicationVersion))
            {
                var assemblyLocation = Assembly.GetCallingAssembly().Location;
                if (assemblyLocation != null)
                {
                    var fileVersionInfo = FileVersionInfo.GetVersionInfo(assemblyLocation);
                    ApplicationVersion = fileVersionInfo.FileVersion;
                }

                return this;
            }

            ApplicationVersion = applicationVersion;
            return this;
        }

        /// <summary>
        /// Include machine name with the request.
        /// </summary>
        /// <param name="machineName">Optional custom machine name. Defaults to <see cref="Environment.MachineName"/></param>
        /// <returns>The current <see cref="Request"/> instance with the machine name</returns>
        public Request WithMachineName(string machineName = null)
        {
            if (string.IsNullOrEmpty(machineName))
            {
                MachineName = Environment.MachineName;
                return this;
            }

            MachineName = machineName;
            return this;
        }

        /// <summary>
        /// Include the Url with the request.
        /// </summary>
        /// <returns>The current <see cref="Request"/> instance with the Url</returns>
        public Request WithUrl()
        {
            var tempUrl = _context.Request.Uri.ToString();
            var tempQueryString = _context.Request.QueryString.ToString();

            // Removes the query string from the url
            if ( !string.IsNullOrWhiteSpace(tempQueryString))
            {
                tempUrl.Replace(tempQueryString, string.Empty);
            }

            Url = tempUrl;
            return this;
        }

        /// <summary>
        /// Include the HttpMethod with the request.
        /// </summary>
        /// <returns>The current <see cref="Request"/> instance with the HttpMethod</returns>
        public Request WithHttpMethod()
        {
            HttpMethod = _context.Request.Method;
            return this;
        }

        /// <summary>
        /// Include the Ip Address with the request.
        /// </summary>
        /// <returns>The current <see cref="Request"/> instance with the Ip Address</returns>
        public Request WithIpAddress()
        {
            IpAddress = _context.Request.RemoteIpAddress;
            return this;
        }

        /// <summary>
        /// Include the query string with the request.
        /// </summary>
        /// <param name="queryString">Optional custom query string</param>
        /// <returns>The current <see cref="Request"/> instance with the query string</returns>
        public Request WithQueryString(QueryString queryString = default(QueryString))
        {
            if (QueryString == default(QueryString))
            {
                QueryString = _context.Request.QueryString;
                return this;
            }

            QueryString = queryString;
            return this;
        }

        /// <summary>
        /// Include the body with the request.
        /// </summary>
        /// <param name="form">Optional custom body</param>
        /// <returns>The current <see cref="Request"/> instance with the body</returns>
        public Request WithBody(string body = null)
        {
            if (body == null)
            {
                // Copy the body to a memory stream
                var stream = new MemoryStream();                
                _context.Request.Body.CopyToAsync(stream);
                
                // Rewind stream
                stream.Position = 0;

                // Read content of stream
                body = new StreamReader(stream).ReadToEnd();
                
                // Rewind stream again and replace the request body with the memory stream,
                // since we have consumed the original stream when we copied it
                stream.Position = 0;
                _context.Request.Body = stream;
            }

            Body = body;
            return this;
        }

        /// <summary>
        /// Include the cookie collection with the request.
        /// </summary>
        /// <param name="cookieCollection">Optional custom cookie collection</param>
        /// <returns>The current <see cref="Request"/> instance with the cookie collection</returns>
        public Request WithCookies(RequestCookieCollection cookieCollection = null)
        {
            if (cookieCollection == null)
            {
                Cookies = _context.Request.Cookies;
                return this;
            }

            Cookies = cookieCollection;
            return this;
        }

        /// <summary>
        /// Include the request headers with the request.
        /// </summary>
        /// <param name="cookieCollection">Optional custom request headers</param>
        /// <returns>The current <see cref="Request"/> instance with the request headers</returns>
        public Request WithRequestHeaders(IHeaderDictionary requestHeaders = null)
        {
            if (requestHeaders == null)
            {
                RequestHeaders = _context.Request.Headers;
                return this;
            }

            RequestHeaders = requestHeaders;
            return this;
        }

        /// <summary>
        /// Include the claims with the request.
        /// </summary>
        /// <param name="claims">Optional custom claims</param>
        /// <returns>The current <see cref="Request"/> instance with the claims</returns>
        public Request WithClaims(IEnumerable<Claim> claims = null)
        {
            if (null == claims)
            {
                if (null == _claimResolver)
                    throw new InvalidOperationException($"{nameof(_claimResolver)} must be set");

                Claims = _claimResolver.GetClaims(_context).ToList();
                return this;
            }

            Claims = claims.ToList();

            return this;
        }

        /// <summary>
        /// Include the username with the request.
        /// </summary>
        /// <param name="username">Optional custom username</param>
        /// <returns>The current <see cref="Request"/> instance with the username</returns>
        public Request WithUsername(string username = null)
        {
            if (!string.IsNullOrWhiteSpace(username))
            {
                Username = username;
                return this;
            }

            if (null == Claims && null == _claimResolver)
                throw new InvalidOperationException($"{nameof(_claimResolver)} must be set");

            Username = (Claims ?? _claimResolver.GetClaims(_context)).FirstOrDefault(x => x.Type == "username")?.Value;
            return this;
        }

    }
}
