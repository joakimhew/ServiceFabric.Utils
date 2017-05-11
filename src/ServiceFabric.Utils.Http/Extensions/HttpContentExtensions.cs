using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ServiceFabric.Utils.Http.Extensions
{
    public static class HttpContentExtensions
    {
        /// <summary>
        /// Read as <see cref="ApiResponseMessage{TExpectedMessageType}"/>.
        /// </summary>
        /// <typeparam name="TExpectedMessageType"></typeparam>
        /// <param name="content">The content.</param>
        /// <returns>
        /// <see cref="ApiResponseMessage{TExpectedMessageType}"/>.
        /// </returns>
        public static async Task<ApiResponseMessage<TExpectedMessageType>> ReadAsApiResponseMessageAsync<TExpectedMessageType>(this HttpContent content)
        {
            var json = await content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ApiResponseMessage<TExpectedMessageType>>(json);
        }

        /// <summary>
        /// Read as <see cref="ApiResponseMessage{TExpectedMessageType}"/> and returns only <typeparamref name="TExpectedMessageType"/>.
        /// </summary>
        /// <typeparam name="TExpectedMessageType"></typeparam>
        /// <param name="content">The content.</param>
        /// <returns>
        /// <typeparamref name="TExpectedMessageType"/>.
        /// </returns>
        public static async Task<TExpectedMessageType> ReadApiResponseMessageAs<TExpectedMessageType>(this HttpContent content)
        {
            var json = await content.ReadAsStringAsync();
            var apiResponseMessage = JsonConvert.DeserializeObject<ApiResponseMessage<TExpectedMessageType>>(json);

            return apiResponseMessage.Message;
        }
    }
}