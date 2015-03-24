using System;
using Newtonsoft.Json;

namespace Swagger.Api.ViewModels
{
	internal class ArrayItems
	{
		[JsonProperty("$ref")]
		public String ArrayType { get; set; }
	}
}