using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ServiceFabric.Utils.Ipc.Http
{
    public static class HttpContentExtensions
    {
        public static async Task<ApiResponseMessage> ReadAsApiResponseMessageAsync(this HttpContent content)
        {
            var json = await content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ApiResponseMessage>(json);
        }
    }
}