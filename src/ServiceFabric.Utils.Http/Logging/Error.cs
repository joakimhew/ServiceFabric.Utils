using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ServiceFabric.Utils.Logging
{
    /// <summary>
    ///  Used to generate an <see cref="Error"/> object containing useful information from an HTTP request
    /// </summary>
    public class Error
    {
        internal const string CollectionErrorKey = "CollectionFetchError";
        private readonly Exception _ex;
        private readonly IOwinContext _context;

        /// <summary>
        /// Creates a new instance of <see cref="Error"/>
        /// </summary>
        /// <param name="context">The <see cref="IOwinContext"/> that caused the error</param>
        public Error(IOwinContext context)
        {
            _context = context;
            Id = Guid.NewGuid();
            Created = DateTime.UtcNow;
        }

        /// <summary>
        /// Creates a new instance of <see cref="Error"/>
        /// </summary>
        /// <param name="ex">The exception to use for the instance</param>
        /// <param name="context">The <see cref="IOwinContext"/> that caused the error</param>
        public Error(Exception ex, IOwinContext context)
        {
            _ex = ex;
            _context = context;

            Id = Guid.NewGuid();
            Created = DateTime.UtcNow;
        }

        public Guid Id { get; set; }
        public string ApplicationName { get; set; }
        public string ApplicationVersion { get; set; }
        public string MachineName { get; set; }
        public DateTime Created { get; set; }
        public string Type { get; set; }
        public string Host { get; set; }
        public string Url { get; set; }
        public string HttpMethod { get; set; }
        public int HttpStatusCode { get; set; }
        public string IpAddress { get; set; }
        public string Source { get; set; }
        public string Message { get; set; }
        public string Detail { get; set; }
        public string Sql { get; set; }
        public int? ErrorHash { get; set; }
        public string ServerVariables { get; set; }
        public QueryString QueryString { get; set; }
        public IFormCollection Form { get; set; }
        public RequestCookieCollection Cookies { get; set; }
        public IHeaderDictionary RequestHeaders { get; set; }

        /// <summary>
        /// All of the current instance of <see cref="Error"/>'s properties serialized using <see cref="CamelCasePropertyNamesContractResolver"/>
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
        /// Include application name with error
        /// </summary>
        /// <param name="applicationName">Optional custom application name. Defaults to calling assembly name</param>
        /// <returns>Current instance of error with application name</returns>
        public Error WithApplicationName(string applicationName = null)
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
        /// Include application version with error
        /// </summary>
        /// <param name="applicationVersion">Optional custom application version. Defaults to FileVersion of assembly</param>
        /// <returns>Current instance of error with application name</returns>
        public Error WithApplicationVersion(string applicationVersion = null)
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
        /// Include machine name with error
        /// </summary>
        /// <param name="machineName">Optional custom machine name. Defaults to <see cref="Environment.MachineName"/></param>
        /// <returns>Current instance of error with machine name</returns>
        public Error WithMachineName(string machineName = null)
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
        /// This is included in <see cref="WithAllExceptionProperties"/>
        /// </summary>
        /// <returns>Instance of Error with type exception</returns>
        public Error WithType(string type = null)
        {
            if (type == null)
            {
                Type = _ex.GetType().Name;
                return this;
            }

            Type = type;
            return this;
        }


        /// <summary>
        /// This is included in <see cref="WithAllExceptionProperties"/>
        /// </summary>
        /// <returns>Current instance of error with exception source</returns>
        public Error WithSource()
        {
            Source = _ex.Source;
            return this;
        }

        /// <summary>
        /// This is included in <see cref="WithAllExceptionProperties"/>
        /// </summary>
        /// <param name="message">Optional custom exception message</param>
        /// <returns>Current instance of error with exception message</returns>
        public Error WithMessage(string message = null)
        {
            if (string.IsNullOrEmpty(message))
            {
                Message = _ex.Message;
                return this;
            }

            Message = message;
            return this;
        }

        /// <summary>
        /// This is included in <see cref="WithAllExceptionProperties"/>
        /// </summary>
        /// <param name="detail">Optional custom exception detail</param>
        /// <returns>Current instance of error with exception detail</returns>
        public Error WithDetail(string detail = null)
        {
            if (string.IsNullOrEmpty(detail))
            {
                Detail = _ex.ToString();
                return this;
            }

            Detail = detail;
            return this;
        }

        /// <summary>
        /// This is included in <see cref="WithAllExceptionProperties"/>
        /// </summary>
        /// <returns>Current instance of error with full stack trace appended to detail property</returns>
        public Error WithFullStackTrace()
        {
            var frames = new StackTrace(fNeedFileInfo: true).GetFrames();
            if (frames != null && frames.Length > 2)
            {
                Detail += "\n\nFull Trace:\n\n" + string.Join("", frames.Skip(2));
            }

            ErrorHash = this.GetHash();
            return this;
        }

        public Error WithHost()
        {
            //TODO: Set Host property
            return this;
        }

        /// <summary>
        /// This is included in <see cref="WithAllContextProperties"/>
        /// </summary>
        /// <returns>Current instance of error with request Uri</returns>
        public Error WithUrl()
        {
            Url = _context.Request.Uri.ToString();
            return this;
        }

        /// <summary>
        /// This is included in <see cref="WithAllContextProperties"/>
        /// </summary>
        /// <returns>Current instance of error with request HttpMethod</returns>
        public Error WithHttpMethod()
        {
            HttpMethod = _context.Request.Method;
            return this;
        }

        /// <summary>
        /// This is inclided in <see cref="WithAllContextProperties"/>
        /// </summary>
        /// <returns>Current instance of error with response StatusCode</returns>
        public Error WithHttpStatusCode(int statusCode = 0)
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
        /// This is included in <see cref="WithAllContextProperties"/>
        /// </summary>
        /// <returns>Current instance of error with request remote IP</returns>
        public Error WithIpAddress()
        {
            IpAddress = _context.Request.RemoteIpAddress;
            return this;
        }

        /// <summary>
        /// This is included in <see cref="WithAllContextProperties"/>
        /// </summary>
        /// <param name="sql">Optional custom sql</param>
        /// <returns>Current instance of error with sql trace (If error is of type SQL)</returns>
        public Error WithSql(string sql = null)
        {
            if (string.IsNullOrEmpty(sql))
            {
                if (!_ex.Data.Contains("SQL"))
                {
                    return this;
                }

                Sql = _ex.Data["SQL"] as string;
                return this;
            }

            Sql = sql;
            return this;
        }

        /// <summary>
        /// This is included in <see cref="WithAllContextProperties"/>
        /// </summary>
        /// <param name="queryString">Optional custom query string</param>
        /// <returns>Current instance of error with query string</returns>
        public Error WithQueryString(QueryString queryString = default(QueryString))
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
        /// This is included in <see cref="WithAllContextProperties"/>
        /// </summary>
        /// <param name="form">Optional custom form</param>
        /// <returns>Current instance of error with form</returns>
        public Error WithForm(IFormCollection form = null)
        {
            if (form == null)
            {
                Form = _context.Request.ReadFormAsync().Result;
                return this;
            }

            Form = form;
            return this;
        }

        /// <summary>
        /// This is included in <see cref="WithAllContextProperties"/>
        /// </summary>
        /// <param name="cookieCollection">Optional custom cookie collection</param>
        /// <returns>Current instance of error with cookie collection</returns>
        public Error WithCookies(RequestCookieCollection cookieCollection = null)
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
        /// This is included in <see cref="WithAllContextProperties"/>
        /// </summary>
        /// <param name="requestHeaders">Optional custom request headers</param>
        /// <returns>Current instance of error with request headers</returns>
        public Error WithRequestHeaders(IHeaderDictionary requestHeaders = null)
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
        /// Includes all context related properties in the current instance of <see cref="Error"/>
        /// </summary>
        /// <returns></returns>
        public Error WithAllContextProperties()
        {
            WithHost();
            WithUrl();
            WithHttpMethod();
            WithHttpStatusCode();
            WithIpAddress();
            WithQueryString();
            WithForm();
            WithCookies();
            WithRequestHeaders();

            return this;
        }

        /// <summary>
        /// Includes all exception related properties in the current instance of <see cref="Error"/>
        /// </summary>
        /// <returns></returns>
        public Error WithAllExceptionProperties()
        {
            WithType();
            WithSource();
            WithMessage();
            WithDetail();
            WithSql();
            WithFullStackTrace();

            return this;
        }


        private int? GetHash()
        {
            if (string.IsNullOrEmpty(Detail))
            {
                return null;
            }

            var result = Detail.GetHashCode();
            if (!string.IsNullOrEmpty(MachineName))
            {
                result = (result * 397) ^ MachineName.GetHashCode();
            }

            return result;
        }

    }
}