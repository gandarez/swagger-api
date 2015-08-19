using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Swagger.Api.ViewModels;

namespace Swagger.Api.Implementations
{
    public class ApiDocApiParametersBuilder
    {
        private const string ParameterRegex = @"{(.*?)}";
        private const string OptionalKey = "optional";
        private const string NameKey = "name";
        private const string DefaultName = "Unknown";
        private const string DescriptionKey = "description";
        private const string TypeKey = "type";
        private const string DefaultType = "integer";
        private const string DefaultDescription = "";

        internal List<ApiDocApiParameters> GetApiDocApiParameters(string url)
        {
            var regex = new Regex(ParameterRegex);
            var result = (from parameter in GetParameterFromUrl(regex, url)
                          select GetApiDocApiParameters(parameter, url)).ToList();
            return result;
        }

        private static ApiDocApiParameters GetApiDocApiParameters(string parameter, string url)
        {
            if (parameter.Contains("="))
            {
                var parts = parameter.Split(';');
                var dictionary = parts.Select(x => x.Split('=')).ToDictionary(x => x[0], x => x[1]);

                return new ApiDocApiParameters
                {
                    Required = !dictionary.ContainsKey(OptionalKey) || dictionary[OptionalKey] == "false",
                    ParamType = GetParamType(parameter, url),
                    Name = dictionary.ContainsKey(NameKey) ? dictionary[NameKey] : DefaultName,
                    Description = dictionary.ContainsKey(DescriptionKey) ? dictionary[DescriptionKey] : DefaultDescription,
                    Type = dictionary.ContainsKey(TypeKey) ? dictionary[TypeKey] : DefaultType
                };
            }

            return new ApiDocApiParameters
            {
                Required = true,
                ParamType = GetParamType(parameter, url),
                Name = parameter.Split(':')[0],
                Description = DefaultDescription,
                Type = GetType(parameter)
            };
        }

        private static IEnumerable<string> GetParameterFromUrl(Regex regex, string url)
        {
            return (from Match m in regex.Matches(url) select m.Groups[1].Value);
        }

        private static string GetType(string stripped)
        {
            return (stripped.Split(':').Count() == 1) ? DefaultType : stripped.Split(':')[1];
        }

        private static string GetParamType(string match, string url)
        {
            return (url.IndexOf(match, StringComparison.Ordinal) < ((url.IndexOf('?') == -1) ? url.Length : url.IndexOf('?')) ? "path" : "query");
        }
    }
}