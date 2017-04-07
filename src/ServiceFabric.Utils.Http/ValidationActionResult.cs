using System.Net;
using System.Net.Http;
namespace ServiceFabric.Utils.Http
{
    public class ValidationActionResult : ApiHttpActionResult
    {
        public ValidationActionResult(HttpRequestMessage request, HttpStatusCode statusCode,
            object message, object additionalInfo = null)
            : base(request, statusCode, message, additionalInfo)
        {
        }

        public static ValidationActionResult NullOrWhiteSpaceField(HttpRequestMessage request, string fieldName)
            => new ValidationActionResult(
                request,
                HttpStatusCode.BadRequest,
                $"'{fieldName}' cannot be null, empty, or consists only of white-space");

        public static ValidationActionResult EmptyIdentifier(HttpRequestMessage request, string identifierFieldName)
            => new ValidationActionResult(
                request,
                HttpStatusCode.BadRequest,
                $"'{identifierFieldName}' cannot be empty");

        public static ValidationActionResult InvalidFieldValue<TValue>(HttpRequestMessage request, string fieldName, TValue value)
            => new ValidationActionResult(
                request,
                HttpStatusCode.BadRequest,
                $"'{fieldName}' cannot be '{value}'");
    }
}
