using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Swagger.Api.ViewModels
{
    internal class SwaggerApiResource
    {
        [JsonProperty("apiVersion")]
        public String ApiVersion { get; set; }

        [JsonProperty("swaggerVersion")]
        public String SwaggerVersion { get; set; }

        [JsonProperty("basePath")]
        public String BasePath { get; set; }

        [JsonProperty("resourcePath")]
        public String ResourcePath { get; set; }

        [JsonProperty("apis")]
        public List<SwaggerApiEndpoint> Apis { get; set; }

        [JsonProperty("models")]
        public Dictionary<String, ApiDocModel> Models { get; set; }
    }
}
