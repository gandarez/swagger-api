using System;
using System.Collections.Generic;
using Swagger.Api.ViewModels;

namespace Swagger.Api.Interfaces
{
	internal interface ISwaggerDocumentationTools
	{
		List<SwaggerApiEndpoint> GetControllerApiEndpoints(Type controllerType);
		Dictionary<String, ApiDocModel> GetControllerModels(Type controllerType);
        Dictionary<ApiDocumentationAttribute, Type> GetClassAttributesAndReturnTypes(Type controllerType);
        IList<ApiDocApiOperationResponseMessage> ApiOperationResponseMessages { get; set; }
	}
}
