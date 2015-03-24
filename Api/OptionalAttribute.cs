using System;

namespace Swagger.Api
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
	public class OptionalAttribute : Attribute
	{
	}
}
