using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Swagger.Api.Interfaces;
using Swagger.Api.ViewModels;

namespace Swagger.Api.Implementations
{
    internal class SwaggerDocumentationTools : ISwaggerDocumentationTools
    {
        private readonly ITypeToStringConverter _typeToStringConverter;
        private readonly IModelsGenerator _modelsGenerator;
        private readonly ApiDocApiParametersBuilder _apiDocApiParametersBuilder = new ApiDocApiParametersBuilder();
        private readonly Dictionary<string, ApiDocModel> _controllerModels = new Dictionary<string, ApiDocModel>();
        public IList<ApiDocApiOperationResponseMessage> ApiOperationResponseMessages { get; set; }

        public SwaggerDocumentationTools()
            : this(new TypeToStringConverter(), new ModelsGenerator()) { }

        public SwaggerDocumentationTools(ITypeToStringConverter typeToStringConverter, IModelsGenerator modelsGenerator)
        {
            _typeToStringConverter = typeToStringConverter;
            _modelsGenerator = modelsGenerator;
        }

        public List<SwaggerApiEndpoint> GetControllerApiEndpoints(Type controllerType)
        {
            var apiDocumentationAttributesAndReturnTypes = GetApiDocumentationAttributesAndReturnTypes(controllerType);

            return apiDocumentationAttributesAndReturnTypes.Select(x => new SwaggerApiEndpoint
            {
                Path = GetPath(x.Key.Url),
                Operations = GetApiOperations(controllerType, x)
            }).ToList();
        }

        private static string GetPath(string url)
        {
            return (url.IndexOf('?') == -1) ? url : url.Substring(0, url.IndexOf('?'));
        }

        private List<ApiDocApiOperations> GetApiOperations(Type controllerType, KeyValuePair<ApiDocumentationAttribute, Type> attributeAndReturnType)
        {
            var responseClass =
                _typeToStringConverter.GetApiOperationFormat(attributeAndReturnType.Key.ReturnType ?? attributeAndReturnType.Value);

            MergeApiResponseMessageTypes();

            return new List<ApiDocApiOperations>
			{
				new ApiDocApiOperations
				{
					ResponseClass = responseClass,
					HttpMethod = attributeAndReturnType.Key.RequestType.ToString().ToUpper(),
					Notes = attributeAndReturnType.Key.Notes,                    
					Nickname = string.Concat(attributeAndReturnType.Key.RequestType.ToString().ToLower(), responseClass),
					Summary = attributeAndReturnType.Key.SummaryDescription,
					Parameters = GetParameters(controllerType, attributeAndReturnType),                    
                    ResponseMessages = GetApiOperationResponseMessages(controllerType, attributeAndReturnType.Key.MethodName)
				}
			};
        }

        private IList<ApiDocApiOperationResponseMessage> GetApiOperationResponseMessages(Type type, string methodName)
        {
            var result = new List<ApiDocApiOperationResponseMessage>();
            result.AddRange(ApiOperationResponseMessages.Select(a => a.Convert()));

            var methodInfo = type.GetMethod(methodName);
            
            (from attribute in GetApiOperationResponseMessageAttributes(methodInfo)
             let customAttribute = (ApiResponseMessageAttribute)attribute
             select customAttribute).ToList().ForEach(a => result.Insert(0, a.ToApiDocApiOperationResponseMessage()));

            return result;
        }

        private void MergeApiResponseMessageTypes()
        {
            foreach (var type in ApiOperationResponseMessages.Where(t => t.Type != null).Select(t => t.Type))
            {
                _controllerModels.Merge(_modelsGenerator.GetModels(type));
            }
        }

        private List<ApiDocApiParameters> GetParameters(Type controllerType, KeyValuePair<ApiDocumentationAttribute, Type> attributeAndReturnType)
        {
            var url = attributeAndReturnType.Key.Url;
            var result = _apiDocApiParametersBuilder.GetApiDocApiParameters(url);

            result.AddRange(from x in controllerType.GetCustomAttributes(typeof(ApiDeclarativeHeaderAttribute), true)
                            let attribute = (ApiDeclarativeHeaderAttribute)x
                            select new ApiDocApiParameters
                            {
                                ParamType = "header",
                                Type = attribute.Type.Name.ToLower(),
                                Name = attribute.Name,
                                Description = attribute.Description,
                                Required = attribute.Required
                            });

            if (attributeAndReturnType.Key.FormBody != null)
            {
                var type = _typeToStringConverter.GetApiOperationFormat(attributeAndReturnType.Key.FormBody);
                var apiModelAttribute = attributeAndReturnType.Key.FormBody.GetCustomAttributes(typeof(ApiModelAttribute), true);

                result.Add(new ApiDocApiParameters
                {
                    ParamType = "body",
                    Type = type,
                    Name = "body",
                    Description = apiModelAttribute.Length > 0 ? ((ApiModelAttribute)apiModelAttribute[0]).Description : null,
                    Required = true
                });
            }
            else
            {
                result.AddRange(
                    (from x in controllerType.GetMethod(url.Split('/').Last()).GetParameters()
                     let isRequired = x.GetCustomAttribute(typeof(OptionalAttribute), true) == null
                     select new ApiDocApiParameters
                     {
                         ParamType = "query",
                         Type = _typeToStringConverter.GetApiOperaiontType(x.ParameterType),
                         Name = x.Name,
                         Description = GetParameterDescription(x),
                         Required = isRequired
                     }));
            }

            return result;
        }

        private static string GetParameterDescription(ParameterInfo parameter)
        {
            var apiModelAttribute = parameter.GetCustomAttributes(typeof(ApiModelAttribute), true);

            return apiModelAttribute.Length > 0 ? ((ApiModelAttribute)apiModelAttribute[0]).Description : string.Empty;
        }

        private Dictionary<ApiDocumentationAttribute, Type> GetApiDocumentationAttributesAndReturnTypes(Type controllerType)
        {
            var result = GetMethodsAttributesAndReturnTypes(controllerType);            

            return result;
        }

        public Dictionary<string, ApiDocModel> GetControllerModels(Type controllerType)
        {
            foreach (var apiDocumentationAttributesAndReturnType in GetApiDocumentationAttributesAndReturnTypes(controllerType))
            {
                if (GetTypesShouldBeExcluded()
                    .Contains(apiDocumentationAttributesAndReturnType.Key.ReturnType)) continue;                

                _controllerModels.Merge(_modelsGenerator.GetModels(apiDocumentationAttributesAndReturnType.Key.ReturnType ?? apiDocumentationAttributesAndReturnType.Value));

                if (apiDocumentationAttributesAndReturnType.Key.FormBody != null)
                    _controllerModels.Merge(_modelsGenerator.GetModels(apiDocumentationAttributesAndReturnType.Key.FormBody));
            }

            return _controllerModels;
        }

        private IEnumerable<MethodInfo> GetControllerMethods(Type controllerType)
        {
            return controllerType.GetMethods();
        }

        private Dictionary<ApiDocumentationAttribute, Type> GetMethodsAttributesAndReturnTypes(Type controllerType)
        {
            var methodInfos = GetControllerMethods(controllerType);

            return (from methodInfo in methodInfos
                    from attribute in GetApiDocumentationAttributes(methodInfo)
                    let customAttribute = (ApiDocumentationAttribute)attribute                    
                    select
                        new KeyValuePair<ApiDocumentationAttribute, Type>(
                            new ApiDocumentationAttribute
                            {
                                MethodName = methodInfo.Name,
                                Url = string.IsNullOrEmpty(customAttribute.Url.Trim())
                                    ? new[] { "GET", "POST", "PUT", "DELETE", "PATCH" }.Contains(methodInfo.Name.ToUpper())
                                        ? string.Format("/{0}",
                                            controllerType.Name.Replace(SwaggerDocumentationCreator.ControllerEnding, ""))
                                        : string.Format("/{0}/{1}",
                                            controllerType.Name.Replace(SwaggerDocumentationCreator.ControllerEnding, ""),
                                            methodInfo.Name)
                                    : customAttribute.Url,
                                SummaryDescription = customAttribute.SummaryDescription,
                                Notes = customAttribute.Notes,
                                ReturnType = customAttribute.ReturnType,
                                RequestType = customAttribute.RequestType,
                                FormBody = customAttribute.FormBody
                            }, methodInfo.ReturnType)
                ).ToDictionary(x => x.Key, x => x.Value);
        }

        private static IEnumerable<Type> GetTypesShouldBeExcluded()
        {
            return new[] {typeof (string), typeof(List<string>)};
        }

        public Dictionary<ApiDocumentationAttribute, Type> GetClassAttributesAndReturnTypes(Type controllerType)
        {
            return (from attr in GetApiDocumentationAttributes(controllerType)
                    let attribute = (ApiDocumentationAttribute)attr
                    select new KeyValuePair<ApiDocumentationAttribute, Type>(attribute, attribute.ReturnType)
                ).ToDictionary(x => x.Key, x => x.Value);
        }

        private static IEnumerable<Attribute> GetApiDocumentationAttributes(MemberInfo controllerType)
        {
            return Attribute.GetCustomAttributes(controllerType, typeof(ApiDocumentationAttribute), false);
        }

        private static IEnumerable<Attribute> GetApiOperationResponseMessageAttributes(MemberInfo controllerType)
        {
            return Attribute.GetCustomAttributes(controllerType, typeof(ApiResponseMessageAttribute), false);
        }
    }
}