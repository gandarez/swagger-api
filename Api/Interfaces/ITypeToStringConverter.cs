using System;

namespace Swagger.Api.Interfaces
{
    internal interface ITypeToStringConverter
    {
        string GetApiOperationFormat(Type typeToConvert);
        string GetApiOperaiontType(Type typeConvert);
    }
}