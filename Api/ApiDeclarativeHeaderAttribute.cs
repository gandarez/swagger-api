using System;

namespace Swagger.Api
{
    /// <summary>
    /// Swagger spec 5.2.4 Parameter Object
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ApiDeclarativeHeaderAttribute : Attribute
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public bool Required { get; private set; }
        public Type Type { get; private set; }

        public ApiDeclarativeHeaderAttribute(string name, string description, Type type = null, bool required = true)
        {
            Name = name;
            Description = description;
            Required = required;
            Type = type ?? typeof (string);
        }
    }
}