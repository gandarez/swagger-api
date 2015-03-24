using System;
using Newtonsoft.Json;
using Swagger.Api.Interfaces;

namespace Swagger.Api.Implementations
{
	internal class JsonSerializer : IJsonSerializer
	{
		public String SerializeObject( Object objectToBeEncoded )
		{
			return JsonConvert.SerializeObject( objectToBeEncoded, Formatting.Indented );
		}
	}
}