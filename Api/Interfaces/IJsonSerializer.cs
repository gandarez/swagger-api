using System;

namespace Swagger.Api.Interfaces
{
	public interface IJsonSerializer
	{
		String SerializeObject( Object objectToBeEncoded );
	}
}
