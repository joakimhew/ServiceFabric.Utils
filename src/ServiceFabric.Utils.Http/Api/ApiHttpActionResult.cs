using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using ServiceFabric.Utils.Http.Extensions;

namespace ServiceFabric.Utils.Api
{
    /// <summary>
    /// Implementation of <see cref="IHttpActionResult"/> to generate <see cref="ApiHttpResponseMessage"/>
    /// </summary>
    public class ApiHttpActionResult : IHttpActionResult
    {
        private readonly HttpRequestMessage _requestMessage;
        private readonly HttpStatusCode _code;
        private readonly object _message;
        private readonly object _info;

        /// <summary>
        /// Creates a new intsance of <see cref="ApiHttpActionResult"/> based on a <see cref="HttpRequestMessage"/>.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequestMessage"/> used to generate the response</param>
        /// <param name="statusCode">The <see cref="HttpStatusCode"/> to set the response status code to</param>
        /// <param name="message">The response message used in <see cref="ApiHttpResponseMessage"/></param>
        /// <param name="additionalInfo">Any additional info that you might want in your <see cref="ApiHttpResponseMessage"/></param>
        public ApiHttpActionResult(HttpRequestMessage request, HttpStatusCode statusCode,
            object message, object additionalInfo = null)
        {
            _requestMessage = request;
            _code = statusCode;
            _message = message;
            _info = additionalInfo;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var responseMessageResult = new ResponseMessageResult(
                _requestMessage.CreateApiResponse(_code, _message, _info));

            var response = await responseMessageResult.ExecuteAsync(cancellationToken);

            return response;
        }
    }
}
