using System;
using Newtonsoft.Json;

namespace Swagger.Api.ViewModels
{
	internal class SwaggerApiSummary
	{
        [JsonProperty("path")]
		public String Path { get; set; }

        [JsonProperty("description")]
		public String Description { get; set; }
	}
}
