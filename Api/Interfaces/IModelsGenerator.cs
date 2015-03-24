using System;
using System.Collections.Generic;
using Swagger.Api.ViewModels;

namespace Swagger.Api.Interfaces
{
    internal interface IModelsGenerator
    {
        Dictionary<String, ApiDocModel> GetModels(Type type);
    }
}