using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ServiceFabric.Utils.Api
{
    /// <summary>
    /// Custom API response message in the form of a camel case JSON object ({ statusCode, message, additionalInfo })
    /// </summary>
    public class ApiHttpResponseMessage : HttpResponseMessage
    {
        /// <summary>
        /// Creates a new instance of <see cref="ApiHttpResponseMessage"/>
        /// </summary>
        /// <param name="statusCode">statusCode of the JSON object</param>
        /// <param name="message">message property of the JSON object</param>
        /// <param name="additionalInfo">additionalInfo property of the JSON object</param>
        public ApiHttpResponseMessage(HttpStatusCode statusCode, object message, object additionalInfo = null)
        {
            var body = new
            {
                code = statusCode,
                message,
                info = additionalInfo
            };

            base.Content = new ObjectContent<object>(body, new JsonMediaTypeFormatter
            {
                SerializerSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }
            });

            base.StatusCode = statusCode;
        }
    }
}