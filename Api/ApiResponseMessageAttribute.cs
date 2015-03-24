using System;
using System.Net;

namespace Swagger.Api
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ApiResponseMessageAttribute : Attribute
    {
        public HttpStatusCode Code { get; private set; }
        public string Message { get; private set; }              
        public Type Type { get; private set; }

        public ApiResponseMessageAttribute(HttpStatusCode code, string message, Type type = null)
        {
            Code = code;
            Message = message;
            Type = type;
        }
    }
}