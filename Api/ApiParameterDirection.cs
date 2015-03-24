using System;
using Swagger.Api.Enum;

namespace Swagger.Api
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ApiParameterDirection : Attribute
    {
        public ModelParameterDirection ParameterDirection { get; set; }

        public ApiParameterDirection(ModelParameterDirection parameterDirection)
        {
            ParameterDirection = parameterDirection;
        }
    }
}