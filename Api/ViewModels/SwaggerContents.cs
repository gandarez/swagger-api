using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Swagger.Api.ViewModels
{
	internal class SwaggerContents
	{
        [JsonProperty("apiVersion")]
		public String ApiVersion { get; set; }

        [JsonProperty("swaggerVersion")]
		public String SwaggerVersion { get; set; }

        [JsonProperty("apis")]
		public List<SwaggerApiSummary> Apis { get; set; }
	}
}