using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ServiceFabricServiceModel;

namespace ServiceFabric.Utils.Http.Extensions
{
    /// <summary>
    /// Extension methods for HttpContent
    /// </summary>
    public static class HttpContentExtensions
    {
        /// <summary>
        /// Reads <see cref="HttpContent"/> as an <see cref="ApiResponseMessage{TMessageType}"/> 
        /// with <see cref="ApiResponseMessage{TMessageType}.Message"/> set to <typeparam name="TExpectedMessageType"/> 
        /// as a synchronous operation
        /// </summary>
        /// <typeparam name="TExpectedMessageType">Expected type of the <see cref="ApiResponseMessage{TMessageType}.Message"/> 
        /// property in <see cref="HttpContent"/></typeparam>
        /// <param name="content"><see cref="HttpContent"/> to read from and deserialize</param>
        /// <returns><see cref="ApiResponseMessage{TMessageType}"/> with the <see cref="ApiResponseMessage{TMessageType}.Message"/> 
        /// property set to <typeparam name="TExpectedMessageType"/></returns>
        public static ApiResponseMessage<TExpectedMessageType> ReadAsApiResponseMessage<TExpectedMessageType>(
            this HttpContent content)
        {
            var json = content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<ApiResponseMessage<TExpectedMessageType>>(json);
        }

        /// <summary>
        /// Reads <see cref="HttpContent"/> as an <see cref="ApiResponseMessage{TMessageType}"/> 
        /// with <see cref="ApiResponseMessage{TMessageType}.Message"/> set to <typeparam name="TExpectedMessageType"/> 
        /// as an asynchronous operation
        /// </summary>
        /// <typeparam name="TExpectedMessageType">Expected type of the <see cref="ApiResponseMessage{TMessageType}.Message"/> 
        /// property in <see cref="HttpContent"/></typeparam>
        /// <param name="content"><see cref="HttpContent"/> to read from and deserialize</param>
        /// <returns><see cref="ApiResponseMessage{TMessageType}"/> with the <see cref="ApiResponseMessage{TMessageType}.Message"/> 
        /// property set to <typeparam name="TExpectedMessageType"/></returns>
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
        public static async Task<TExpectedMessageType> ReadApiResponseMessageAs<TExpectedMessageType>(
            this HttpContent content)
        {
            var json = await content.ReadAsStringAsync();
            var apiResponseMessage = JsonConvert.DeserializeObject<ApiResponseMessage<TExpectedMessageType>>(json);

            return apiResponseMessage.Message;
        }

        /// <summary>
        /// Tries to read <see cref="HttpContent"/> as an <see cref="ApiResponseMessage{TMessageType}"/> 
        /// with <see cref="ApiResponseMessage{TMessageType}.Message"/> set to <typeparam name="TExpectedMessageType"/>
        /// as a synchronous operation
        /// </summary>
        /// <typeparam name="TExpectedMessageType"></typeparam>
        /// <param name="content"></param>
        /// <param name="apiResponseMessage"></param>
        /// <returns><see cref="ApiResponseMessage{TMessageType}"/> with the <see cref="ApiResponseMessage{TMessageType}.Message"/> 
        /// property set to <typeparam name="TExpectedMessageType"/></returns>
        public static bool TryReadAsApiResponseMessage<TExpectedMessageType>(
            this HttpContent content, 
            out ApiResponseMessage<TExpectedMessageType> apiResponseMessage) where TExpectedMessageType : new()
        {
            var json = content.ReadAsStringAsync().Result;
            var parsed = json.TryParse<ApiResponseMessage<TExpectedMessageType>>();

            if (parsed == null)
            {
                apiResponseMessage = default(ApiResponseMessage<TExpectedMessageType>);
                return false;
            }

            apiResponseMessage = parsed;
            return true;
        }

        /// <summary>
        /// Tries to read <see cref="HttpContent"/> as an <see cref="ApiResponseMessage{TMessageType}"/> 
        /// with <see cref="ApiResponseMessage{TMessageType}.Message"/> set to <typeparam name="TExpectedMessageType"/>
        /// as an asynchronous operation
        /// </summary>
        /// <typeparam name="TExpectedMessageType"></typeparam>
        /// <param name="content"></param>
        /// <returns><see cref="ApiResponseMessage{TMessageType}"/> with the <see cref="ApiResponseMessage{TMessageType}.Message"/> 
        /// property set to <typeparam name="TExpectedMessageType"/></returns>
        public static async Task<(bool Success, ApiResponseMessage<TExpectedMessageType> ExpectedApiResponseMessage)> 
            TryReadAsApiResponseMessageAsync<TExpectedMessageType>(this HttpContent content) where TExpectedMessageType : new()
        {
            var json = await content.ReadAsStringAsync();

            var parsed = json.TryParse<ApiResponseMessage<TExpectedMessageType>>();

            return parsed == null ? (false, null) : (true, parsed);
        }
    }
}