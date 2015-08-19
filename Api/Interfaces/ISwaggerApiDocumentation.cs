using System;
using System.Collections.Generic;
using Swagger.Api.ViewModels;

namespace Swagger.Api.Interfaces
{
	public interface ISwaggerApiDocumentation
	{
		string GetSwaggerApiList();
		string GetControllerDocumentation(Type controllerType, string baseUrl );
	    void AddApiOperationResponseMessages(IList<ApiDocApiOperationResponseMessage> apiOperationResponseMessages);
	}
}