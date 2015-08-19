using System;
using System.Collections.Generic;
using System.Linq;
using Swagger.Api.ViewModels;

namespace Swagger.Api.Implementations
{
    internal static class Helpers
    {
        public static void Merge<TKey, TValue>(this IDictionary<TKey, TValue> currentDictionary, IDictionary<TKey, TValue> dictionaryToMerge)
        {
            dictionaryToMerge.ToList().ForEach(x =>
            {
                if (!currentDictionary.Keys.Contains(x.Key))
                    currentDictionary.Add(x.Key, x.Value);
            });
        }

        public static ApiDocApiOperationResponseMessage Convert(this ApiDocApiOperationResponseMessage obj)
        {
            return new ApiDocApiOperationResponseMessage
            {
                Code = obj.HttpCode.GetValue<int>(),
                Message = obj.Message,
                ResponseModel =
                    obj.Type != null ? new TypeToStringConverter().GetApiOperationFormat(obj.Type) : string.Empty
            };
        }

        public static ApiDocApiOperationResponseMessage ToApiDocApiOperationResponseMessage(
            this ApiResponseMessageAttribute apiResponseMessageAttribute)
        {
            return
                new ApiDocApiOperationResponseMessage
                {
                    HttpCode = apiResponseMessageAttribute.Code,
                    Message = apiResponseMessageAttribute.Message,
                    Type = apiResponseMessageAttribute.Type
                }.Convert();
        }

        public static T GetValue<T>(this System.Enum enumeration)
        {
            var result = default(T);

            try
            {
                result = (T)System.Convert.ChangeType(enumeration, typeof(T));
            }
            catch (Exception)
            {
                // ignored
            }

            return result;
        }
    }
}