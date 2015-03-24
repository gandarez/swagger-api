using System;
using Newtonsoft.Json;

namespace Swagger.Api.ViewModels
{
	internal class ApiDocApiParameters
	{
        [JsonProperty("name")]
		public String Name { get; set; }

        [JsonProperty("description")]
		public String Description { get; set; }

        [JsonProperty("required")]
		public Boolean Required { get; set; }

        [JsonProperty("type")]
		public String Type { get; set; }

        [JsonProperty("paramType")]
		public String ParamType { get; set; }
	}
}
