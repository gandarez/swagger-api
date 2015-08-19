using Newtonsoft.Json;
using Swagger.Api.Interfaces;

namespace Swagger.Api.Implementations
{
    internal class JsonSerializer : IJsonSerializer
    {
        public string SerializeObject(object objectToBeEncoded)
        {
            return JsonConvert.SerializeObject(objectToBeEncoded, Formatting.Indented);
        }
    }
}