using System;
using System.Net;
using Newtonsoft.Json;

namespace Swagger.Api.ViewModels
{
    public class ApiDocApiOperationResponseMessage
    {
        [JsonProperty("code")]
        internal int Code { get; set; }

        [JsonIgnore]
        public HttpStatusCode HttpCode { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("responseModel")]
        internal String ResponseModel { get; set; }

        [JsonIgnore]
        public Type Type { get; set; }        
    }
}