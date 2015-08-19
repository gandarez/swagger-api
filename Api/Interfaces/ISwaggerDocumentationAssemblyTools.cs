using System;
using System.Collections.Generic;

namespace Swagger.Api.Interfaces
{
    internal interface ISwaggerDocumentationAssemblyTools
    {
        List<Type> GetTypesThatAreDecoratedWithApiDocumentationAttribute(IEnumerable<Type> controllerTypes);
        List<Type> GetApiControllerTypes(Type baseControllerType);
    }
}