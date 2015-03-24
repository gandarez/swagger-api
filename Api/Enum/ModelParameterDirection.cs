using System;
using System.ComponentModel;

namespace Swagger.Api.Enum
{
    [Flags]
    public enum ModelParameterDirection
    {
        [Description("Input")]
        Input,
        [Description("Output")]
        Output,
        [Description("Input/Output")]
        InputOutput
    }
}