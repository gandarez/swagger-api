using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Swagger.Api.ViewModels
{
    internal class ApiDocApiOperations
	{       
        [JsonProperty("method")]
		public String HttpMethod { get; set; }

        [JsonProperty("summary")]
        public String Summary { get; set; }

        [JsonProperty("notes")]
		public String Notes { get; set; }

        [JsonProperty("responseClass")]
		public String ResponseClass { get; set; }

        [JsonProperty("nickname")]
		public String Nickname { get; set; }

        [JsonProperty("parameters")]
		public List<ApiDocApiParameters> Parameters { get; set; }

        [JsonProperty("responseMessages")]
        public IList<ApiDocApiOperationResponseMessage> ResponseMessages { get; set; }
	}
}
