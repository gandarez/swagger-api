using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Swagger.Api.ViewModels
{
	internal class ApiDocModel
	{
        [JsonProperty("id")]
		public String Id { get; set; }

        [JsonProperty("properties")]
		public Dictionary<String, ApiDocModelProperty> Properties { get; set; }

        [JsonProperty("required")]
		public List<String> Required { get; set; }
	}
}