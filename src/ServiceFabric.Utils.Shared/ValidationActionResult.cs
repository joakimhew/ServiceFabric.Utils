using System.Net;
using System.Net.Http;

namespace ServiceFabric.Utils.Shared
{
    public class ValidationActionResult : ApiHttpActionResult
    {
        public ValidationActionResult(HttpRequestMessage request, HttpStatusCode statusCode,
            object message, object additionalInfo = null)
            : base(request, statusCode, message, additionalInfo)
        {
        }

        /// <summary>
        /// Create a new instance of <see cref="ValidationActionResult"/> for a field that is null, empty or consists only of white-space.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="fieldName">The name of the field.</param>
        /// <returns>A new instance of <see cref="ValidationActionResult"/>.</returns>
        public static ValidationActionResult NullOrWhiteSpaceField(HttpRequestMessage request, string fieldName)
            => new ValidationActionResult(
                request,
                HttpStatusCode.BadRequest,
                $"'{fieldName}' cannot be null, empty, or consists only of white-space");

        /// <summary>
        /// Create a new instance of <see cref="ValidationActionResult"/> for an identifier that is empty or invalid.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="identifierFieldName">The name of the identifier.</param>
        /// <returns>A new instance of <see cref="ValidationActionResult"/>.</returns>
        public static ValidationActionResult EmptyIdentifier(HttpRequestMessage request, string identifierFieldName)
            => new ValidationActionResult(
                request,
                HttpStatusCode.BadRequest,
                $"'{identifierFieldName}' cannot be empty");

        /// <summary>
        /// Create a new instance of <see cref="ValidationActionResult"/> for a field with invalid value.
        /// </summary>
        /// <typeparam name="TValue">The value of the invalid field.</typeparam>
        /// <param name="request">The request.</param>
        /// <param name="fieldName">The name of the field.</param>
        /// <returns>A new instance of <see cref="ValidationActionResult"/>.</returns>
        public static ValidationActionResult InvalidFieldValue<TValue>(HttpRequestMessage request, string fieldName, TValue value)
            => new ValidationActionResult(
                request,
                HttpStatusCode.BadRequest,
                $"'{fieldName}' cannot be '{value}'");

        /// <summary>
        /// Create a new instance of <see cref="ValidationActionResult"/> with a custom message.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="fieldName">The name of the field.</param>
        /// <returns>A new instance of <see cref="ValidationActionResult"/>.</returns>
        public static ValidationActionResult CustomMessage(HttpRequestMessage request, string message)
           => new ValidationActionResult(
               request,
               HttpStatusCode.BadRequest,
               message);
    }
}
