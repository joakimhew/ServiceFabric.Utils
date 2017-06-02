using Microsoft.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Diagnostics;
using System.Reflection;
namespace ServiceFabric.Utils.Http.Logging
{
    /// <summary>
    /// Log data of a response
    /// </summary>
    public class Response
    {
        private readonly IOwinContext _context;

        /// <summary>
        /// Creates a new instance of <see cref="Response"/> with the specified <see cref="IOwinContext"/>.
        /// </summary>
        /// <param name="context">The <see cref="<see cref="IOwinContext"/></param>
        public Response(IOwinContext context)
        {
            _context = context;
            this.Id = Guid.NewGuid();
            this.Created = DateTime.UtcNow;
        }

        /// <summary>
        /// Creates a new instance of <see cref="Response"/> with the specified <see cref="IOwinContext"/>.
        /// </summary>
        /// <param name="context">The <see cref="<see cref="IOwinContext"/></param>
        /// <param name="requestId">The <see cref="Guid"/> of the associated request</param>
        public Response(IOwinContext context, Guid requestId) 
            : this(context)
        {
            this.RequestId = requestId;
        }

        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Gets or sets the Request Id.
        /// </summary>
        public Guid? RequestId { get; set; }
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
        /// Gets or sets the Http Status Code.
        /// </summary>
        public int HttpStatusCode { get; set; }
        /// <summary>
        /// Gets or sets the Response Cookies.
        /// </summary>
        public ResponseCookieCollection Cookies { get; set; }
        /// <summary>
        /// Gets or sets the Response Headers.
        /// </summary>
        public IHeaderDictionary ResponseHeaders { get; set; }
        /// <summary>
        /// Gets or sets the Elapsed time in Milliseconds.
        /// </summary>
        public long ElapsedMilliseconds { get; set; }

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
        /// Include application name with the response.
        /// </summary>
        /// <param name="applicationName">Optional custom application name. Defaults to calling assembly name</param>
        /// <returns>The current <see cref="Response"/> instance with the application name</returns>
        public Response WithApplicationName(string applicationName = null)
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
        /// Include application version with the response.
        /// </summary>
        /// <param name="applicationVersion">Optional custom application version. Defaults to FileVersion of assembly</param>
        /// <returns>The current <see cref="Response"/> instance with the application version</returns>
        public Response WithApplicationVersion(string applicationVersion = null)
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
        /// Include machine name with the response.
        /// </summary>
        /// <param name="machineName">Optional custom machine name. Defaults to <see cref="Environment.MachineName"/></param>
        /// <returns>The current <see cref="Response"/> instance with the machine name</returns>
        public Response WithMachineName(string machineName = null)
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
        /// Includes the Http Status Code with the response.
        /// </summary>
        /// <param name="statusCode">Optional Http Status Code</param>
        /// <returns>The current <see cref="Response"/> instance with the Http Status Code</returns>
        public Response WithHttpStatusCode(int statusCode = 0)
        {
            if (statusCode == 0)
            {
                HttpStatusCode = _context.Response.StatusCode;
                return this;
            }
            HttpStatusCode = statusCode;
            return this;
        }

        /// <summary>
        /// Include the request headers with the response.
        /// </summary>
        /// <param name="cookieCollection">Optional custom request headers</param>
        /// <returns>The current <see cref="Response"/> instance with the request headers</returns>
        public Response WithResponseHeaders(IHeaderDictionary responseHeaders = null)
        {
            if (responseHeaders == null)
            {
                ResponseHeaders = new HeaderDictionary( _context.Response.Headers );
                return this;
            }

            ResponseHeaders = responseHeaders;
            return this;
        }
    }
}
