using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;

namespace ServiceFabric.Utils.Http.Extensions
{
    public static class NewtonsoftJsonExtensions
    {
        public static T TryParse<T>(this string json) where T : new()
        {
            var generator = new JSchemaGenerator();
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