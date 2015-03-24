using System;

namespace Swagger.Api
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Parameter)]
    public class ApiModelAttribute : Attribute
    {
        public String Description { get; private set; }

        public ApiModelAttribute(String description = "")
        {
            Description = description;
        }
    }
}