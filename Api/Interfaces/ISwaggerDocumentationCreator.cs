using System;
using System.Collections.Generic;
using Swagger.Api.ViewModels;

namespace Swagger.Api.Interfaces
{
    internal interface ISwaggerDocumentationCreator
    {
        SwaggerContents GetSwaggerResourceList(IEnumerable<Type> controllerTypes);
        SwaggerApiResource GetApiResource(Type controllerType, string baseUrl);
        void AddApiOperationResponseMessages(IList<ApiDocApiOperationResponseMessage> apiOperationResponseMessages);
    }
}