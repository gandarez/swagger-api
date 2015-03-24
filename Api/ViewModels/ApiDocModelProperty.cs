using System;
using Newtonsoft.Json;

namespace Swagger.Api.ViewModels
{
	internal class ApiDocModelProperty
	{
        [JsonProperty("type")]
		public String Type { get; set; }

        [JsonProperty("format")]
	    public String Format { get; set; }

        [JsonProperty("minimum")]
        public String Minimum { get; set; }

        [JsonProperty("maximum")]
        public String Maximum { get; set; }

        [JsonProperty("description")]
        public String Description { get; set; }

        [JsonProperty("items")]
		public ArrayItems Items { get; set; }

        [JsonProperty("$ref")]
        public String ComplexType { get; set; }

        [JsonProperty("enum")]
        public String[] Enum { get; set; }
	}
}
