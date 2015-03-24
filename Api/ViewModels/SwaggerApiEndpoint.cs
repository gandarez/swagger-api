using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Swagger.Api.ViewModels
{
	internal class SwaggerApiEndpoint
	{
        [JsonProperty("path")]
		public String Path { get; set; }

        [JsonProperty("operations")]
		public List<ApiDocApiOperations> Operations { get; set; }
	}
}
