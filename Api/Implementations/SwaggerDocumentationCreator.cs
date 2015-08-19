using System;
using System.Collections.Generic;
using System.Linq;
using Swagger.Api.Interfaces;
using Swagger.Api.ViewModels;

namespace Swagger.Api.Implementations
{    
    internal class SwaggerDocumentationCreator : ISwaggerDocumentationCreator
    {
        private readonly string _apiVersion;
        private const string SwaggerVersion = "1.2";
        internal const string ControllerEnding = "Controller";        

        private readonly ISwaggerDocumentationTools _swaggerDocumentationTools;

        public SwaggerDocumentationCreator(string apiVersion)
            : this(new SwaggerDocumentationTools())
        {
            _apiVersion = string.IsNullOrEmpty(apiVersion.Trim()) ? "1.0" : apiVersion;            
        }

        public SwaggerDocumentationCreator(ISwaggerDocumentationTools swaggerDocumentationTools)
        {
            _swaggerDocumentationTools = swaggerDocumentationTools;
        }

        public SwaggerContents GetSwaggerResourceList(IEnumerable<Type> controllerTypes)
        {
            return new SwaggerContents
            {
                ApiVersion = _apiVersion,
                SwaggerVersion = SwaggerVersion,
                Apis = GetSwaggerApiSummaries(controllerTypes)
            };
        }

        public void AddApiOperationResponseMessages(
            IList<ApiDocApiOperationResponseMessage> apiOperationResponseMessages)
        {
            _swaggerDocumentationTools.ApiOperationResponseMessages = apiOperationResponseMessages;
        }

        public SwaggerApiResource GetApiResource(Type controllerType, string baseUrl)
        {
            return new SwaggerApiResource
            {
                ApiVersion = _apiVersion,
                SwaggerVersion = SwaggerVersion,
                BasePath = baseUrl,
                ResourcePath = GetControllerPath(controllerType),
                Apis = _swaggerDocumentationTools.GetControllerApiEndpoints(controllerType),
                Models = _swaggerDocumentationTools.GetControllerModels(controllerType)
            };
        }

        private List<SwaggerApiSummary> GetSwaggerApiSummaries(IEnumerable<Type> controllerTypes)
        {
            var array = controllerTypes.Select(GetSwaggerApiSummary)
                                       .ToArray();

            Array.Sort(array, SwaggerApiSummariesComparison);

            return array.ToList();            
        }

        private static int SwaggerApiSummariesComparison(SwaggerApiSummary o1, SwaggerApiSummary o2)
        {
            //String to order controllers ((H)ome, (E)xample, etc..)
            const string order = "HE";

            var pos1 = 0;
            var pos2 = 0;
            for (var i = 0; i < Math.Min(o1.Path.Length, o2.Path.Length) && pos1 == pos2; i++)
            {              
                pos1 = order.IndexOf(o1.Path.ToUpper().ToCharArray()[i]);
                pos2 = order.IndexOf(o2.Path.ToUpper().ToCharArray()[i]);
            }

            return pos1 - pos2;
        }

        private SwaggerApiSummary GetSwaggerApiSummary(Type controllerType)
        {
            return new SwaggerApiSummary
            {
                Path = GetControllerPath(controllerType),
                Description = GetControllerDescription(controllerType)
            };
        }

        private static string GetControllerPath(Type controllerType)
        {           
            return string.Format("/{0}", controllerType.Name.Replace(ControllerEnding, string.Empty));
        }

        private string GetControllerDescription(Type controllerType)
        {
            var ret = _swaggerDocumentationTools.GetClassAttributesAndReturnTypes(controllerType);            

            return ret != null ? ret.First().Key.SummaryDescription : string.Empty;
        }
    }
}