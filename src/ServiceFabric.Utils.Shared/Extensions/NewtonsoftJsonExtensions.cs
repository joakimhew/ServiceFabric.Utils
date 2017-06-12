using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Serialization;

namespace ServiceFabric.Utils.Shared.Extensions
{
    public static class NewtonsoftJsonExtensions
    {
        public static T TryParse<T>(this string json)
        {
            return TryParse<T>(json, new CamelCasePropertyNamesContractResolver());
        }

        public static T TryParse<T>(this string json, IContractResolver contractResolver)
        {
            var generator = new JSchemaGenerator
            {
                ContractResolver = contractResolver
            };

            var schema = generator.Generate(typeof(T));

            try
            {
                var jObject = JObject.Parse(json);
                return jObject.IsValid(schema) ? JsonConvert.DeserializeObject<T>(json) : default(T);
            }

            catch
            {
                return default(T);
            }
        }
    }
}