using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Swagger.Api.Enum;
using Swagger.Api.Interfaces;
using Swagger.Api.Toolbox;
using Swagger.Api.ViewModels;

namespace Swagger.Api.Implementations
{
    internal class ModelsGenerator : IModelsGenerator
    {
        private readonly ITypeToStringConverter _typeToStringConverter;

        public ModelsGenerator() : this(new TypeToStringConverter()) { }

        public ModelsGenerator(ITypeToStringConverter typeToStringConverter)
        {
            _typeToStringConverter = typeToStringConverter;
        }

        private readonly List<Type> _processedTypes = new List<Type>();

        private readonly List<Type> _exclusions = new List<Type>
		{
			typeof( String ),
			typeof( Type ),
			typeof( DateTime ),
			typeof( TimeSpan ),
			typeof( Decimal ),
			typeof( Boolean )
		};

        public virtual Dictionary<String, ApiDocModel> GetModels(Type type)
        {
            if (IsAList(type))
                type = GetGenericArgument(type, 0);
            if (IsArray(type))
                type = type.GetElementType();

            var name = _typeToStringConverter.GetApiOperationFormat(type);
            var apiDocModels = InitializeApiDocModels(name);

            ProcessType(type, apiDocModels);

            return apiDocModels;
        }

        private void ProcessType(Type type, Dictionary<string, ApiDocModel> apiDocModels)
        {
            foreach (var property in type.GetProperties().Where(p => !ShouldIgnore(p)))
            {
                var propertyType = property.PropertyType;
                var modelProperty = DefaultModelProperty(property);                         
                
                if (IsAList(propertyType))
                {
                    propertyType = GetGenericArgument(propertyType, 0);
                    modelProperty = GetArrayModelProperty(propertyType);
                }
                else if (IsArray(propertyType))
                {
                    propertyType = propertyType.GetElementType();
                    modelProperty = GetArrayModelProperty(propertyType);
                }
                else if (IsDictionary(propertyType))
                {
                    var key = GetGenericArgument(propertyType, 0);
                    var value = GetGenericArgument(propertyType, 1);

                    GetNonPrimitiveModels(key, apiDocModels);
                    GetNonPrimitiveModels(value, apiDocModels);

                    apiDocModels.Merge(GetKeyValuePairModel(key, value));

                    modelProperty = GetArrayModelProperty(typeof(KeyValuePair<,>));
                }
                else if (propertyType.IsClass)
                {
                    modelProperty = GetComplexTypeModelProperty(propertyType);
                }                

                GetNonPrimitiveModels(propertyType, apiDocModels);
                apiDocModels.First().Value.Properties.Add(property.Name, modelProperty);

                GetOptionalArgument(property, apiDocModels);
                GetModelArgument(property, apiDocModels);
                GetRangeArgument(property, apiDocModels);
                GetEnumArgument(property, apiDocModels);
            }
        }

        private static void GetOptionalArgument(PropertyInfo property, Dictionary<string, ApiDocModel> apiDocModels)
        {
            if (property.GetCustomAttributes(typeof(OptionalAttribute), true).Length == 0)
                apiDocModels.First().Value.Required.Add(property.Name);
        }



        private static void GetModelArgument(PropertyInfo property, Dictionary<string, ApiDocModel> apiDocModels)
        {
            var apiModelAttribute = property.GetCustomAttribute(typeof(ApiModelAttribute), true);
            if (apiModelAttribute != null)
                apiDocModels.First()
                            .Value.Properties[property.Name].Description = ((ApiModelAttribute)apiModelAttribute).Description;

            var apiParameterTypeAttribute = property.GetCustomAttribute(typeof(ApiParameterDirection), true);
            if (apiParameterTypeAttribute != null)
            {
                var attribute = ((ApiParameterDirection)apiParameterTypeAttribute).ParameterDirection;
                apiDocModels.First().Value.Properties[property.Name].Description =
                    string.Format("{0} [{1}]",
                        apiDocModels.First()
                                    .Value.Properties[property.Name].Description,
                        new DescribedEnum<ModelParameterDirection>(attribute));
            }
        }

        private static void GetRangeArgument(PropertyInfo property, Dictionary<string, ApiDocModel> apiDocModels)
        {
            var apiRangeAttribute = property.GetCustomAttribute(typeof(ApiRangeAttribute), true);
            if (apiRangeAttribute != null)
            {
                apiDocModels.First()
                            .Value.Properties[property.Name].Minimum =
                    ((ApiRangeAttribute)apiRangeAttribute).Minimum.ToString(CultureInfo.InvariantCulture);
                apiDocModels.First()
                            .Value.Properties[property.Name].Maximum =
                    ((ApiRangeAttribute)apiRangeAttribute).Maximum.ToString(CultureInfo.InvariantCulture);
            }
        }

        private static void GetEnumArgument(PropertyInfo property, Dictionary<string, ApiDocModel> apiDocModels)
        {
            var apiEnumAttribute = property.GetCustomAttribute(typeof(ApiEnumAttribute), true);
            if (apiEnumAttribute != null)
                apiDocModels.First()
                            .Value.Properties[property.Name].Enum =
                    ((ApiEnumAttribute)apiEnumAttribute).Enum.ToArray();
        }

        private Type GetGenericArgument(Type type, int index)
        {
            return type.GetGenericArguments()[index];
        }

        private ApiDocModelProperty DefaultModelProperty(PropertyInfo property)
        {
            return new ApiDocModelProperty
            {
                //TODO: format and type must follow below rules
                //https://github.com/wordnik/swagger-spec/blob/master/versions/1.2.md#431-primitives
                Format = _typeToStringConverter.GetApiOperationFormat(property.PropertyType),
                Type = _typeToStringConverter.GetApiOperaiontType(property.PropertyType)
            };
        }

        private ApiDocModelProperty GetArrayModelProperty(Type arrayType)
        {
            return new ApiDocModelProperty
            {
                Type = "array",
                Items = new ArrayItems
                {
                    ArrayType = _typeToStringConverter.GetApiOperationFormat(arrayType)
                }
            };
        }

        private ApiDocModelProperty GetComplexTypeModelProperty(Type complexType)
        {
            return new ApiDocModelProperty
            {
                Type = "",
                ComplexType = _typeToStringConverter.GetApiOperationFormat(complexType)
            };
        }

        private static bool IsDictionary(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
        }

        private static bool IsArray(Type type)
        {
            return type.IsArray;
        }

        private static bool IsAList(Type type)
        {
            return type.IsGenericType &&
                   (type.GetGenericTypeDefinition() == typeof(List<>) ||
                    type.GetGenericTypeDefinition() == typeof(IList<>) ||
                    type.GetGenericTypeDefinition() == typeof(Collection<>) ||
                    type.GetGenericTypeDefinition() == typeof(ICollection<>) ||
                    type.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        }

        private static bool ShouldIgnore(PropertyInfo property)
        {
            var attribute = property.GetCustomAttribute(typeof(ApiIgnoreAttribute), false);

            return attribute != null;
        }

        private void GetNonPrimitiveModels(Type propertyType, IDictionary<string, ApiDocModel> apiDocModels)
        {
            if (TypeShouldBeProcessed(propertyType) && TypeHasNotAlreadyBeenProcessed(propertyType))
            {
                _processedTypes.Add(propertyType);
                apiDocModels.Merge(GetModels(propertyType));
            }
        }

        private bool TypeHasNotAlreadyBeenProcessed(Type propertyType)
        {
            return !_processedTypes.Contains(propertyType);
        }

        private bool TypeShouldBeProcessed(Type propertyType)
        {
            //return !propertyType.IsPrimitive && !_exclusions.Contains(propertyType);
            return propertyType.IsClass && !_exclusions.Contains(propertyType);
        }

        private Dictionary<String, ApiDocModel> GetKeyValuePairModel(Type key, Type value)
        {
            return new Dictionary<String, ApiDocModel>
			{
				{
					"KeyValuePair", new ApiDocModel
					{
						Id = "KeyValuePair",
						Properties = new Dictionary<String, ApiDocModelProperty>
						{
							{
								"Key", new ApiDocModelProperty
								{
									Type = _typeToStringConverter.GetApiOperationFormat( key )
								}
							},
							{
								"Value", new ApiDocModelProperty
								{
									Type = _typeToStringConverter.GetApiOperationFormat( value )
								}
							}
						}
					}
				}
			};
        }

        private static Dictionary<String, ApiDocModel> InitializeApiDocModels(string name)
        {
            return new Dictionary<String, ApiDocModel>
			{
				{
					name, new ApiDocModel
					{
						Id = name,
						Properties = new Dictionary<String, ApiDocModelProperty>(),
						Required = new List<String>()
					}
				}
			};
        }
    }
}