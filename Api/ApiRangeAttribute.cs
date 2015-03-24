using System;

namespace Swagger.Api
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ApiRangeAttribute : Attribute
    {
        public int Minimum { get; set; }
        public int Maximum { get; set; }

        public ApiRangeAttribute(int minimum, int maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
        }
    }
}