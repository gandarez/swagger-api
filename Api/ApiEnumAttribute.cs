using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Swagger.Api
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ApiEnumAttribute : Attribute
    {        
        public ICollection<string> Enum { get; set; }

        public ApiEnumAttribute(params string[] p)
        {
            Enum = new Collection<string>();
            foreach (var item in p)
            {
                Enum.Add(item);
            }
        }        
    }
}