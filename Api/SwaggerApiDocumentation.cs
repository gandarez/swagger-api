using System;
using System.Collections.Generic;
using System.Web.Http;
using Swagger.Api.Implementations;
using Swagger.Api.Interfaces;
using Swagger.Api.ViewModels;

namespace Swagger.Api
{
    public class SwaggerApiDocumentation<TBaseApiControllerType> : ISwaggerApiDocumentation
        where TBaseApiControllerType : ApiController
    {
        private readonly ISwaggerDocumentationAssemblyTools _swaggerDocumentationAssemblyTools;
        private readonly ISwaggerDocumentationCreator _swaggerDocumentationCreator;
        private readonly IJsonSerializer _jsonSerializer;                

        public SwaggerApiDocumentation()
            : this(new SwaggerDocumentationAssemblyTools(), new SwaggerDocumentationCreator(string.Empty), new JsonSerializer()) { }

        public SwaggerApiDocumentation(string apiVersion)
            : this(new SwaggerDocumentationAssemblyTools(), new SwaggerDocumentationCreator(apiVersion), new JsonSerializer()) { }

        public SwaggerApiDocumentation(IJsonSerializer jsonSerializer)
            : this(new SwaggerDocumentationAssemblyTools(), new SwaggerDocumentationCreator(string.Empty), jsonSerializer) { }

        internal SwaggerApiDocumentation(
            ISwaggerDocumentationAssemblyTools swaggerDocumentationAssemblyTools,
            ISwaggerDocumentationCreator swaggerDocumentationCreator,
            IJsonSerializer jsonSerializer)
        {            
            _swaggerDocumentationAssemblyTools = swaggerDocumentationAssemblyTools;
            _swaggerDocumentationCreator = swaggerDocumentationCreator;
            _jsonSerializer = jsonSerializer;
        }

        public void AddApiOperationResponseMessages(
            IList<ApiDocApiOperationResponseMessage> apiOperationResponseMessages)
        {
            _swaggerDocumentationCreator.AddApiOperationResponseMessages(apiOperationResponseMessages);
        }

        public string GetSwaggerApiList()
        {
            var allApiControllers = _swaggerDocumentationAssemblyTools.GetApiControllerTypes(typeof(TBaseApiControllerType));
            var pertinentApiControllers = _swaggerDocumentationAssemblyTools.GetTypesThatAreDecoratedWithApiDocumentationAttribute(allApiControllers);
            var swaggerContents = _swaggerDocumentationCreator.GetSwaggerResourceList(pertinentApiControllers);

            return _jsonSerializer.SerializeObject(swaggerContents);
        }

        public string GetControllerDocumentation(Type controllerType, string baseUrl)
        {
            var apiResource = _swaggerDocumentationCreator.GetApiResource(controllerType, baseUrl);
            return _jsonSerializer.SerializeObject(apiResource);
        }
    }
}