using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Swagger.Api.Interfaces;

namespace Swagger.Api.Implementations
{
    static class TypeSwitch
    {
        public class CaseInfo
        {
            public bool IsDefault { get; set; }
            public Type Target { get; set; }
            public Action<object> Action { get; set; }
        }

        public static void Do(Type type, params CaseInfo[] cases)
        {
            foreach (var entry in cases.Where(entry => entry.IsDefault || entry.Target.IsAssignableFrom(type)))
            {
                entry.Action(type);
                break;
            }
        }

        public static CaseInfo Case<T>(Action action)
        {
            return new CaseInfo
            {
                Action = x => action(),
                Target = typeof(T)
            };
        }

        public static CaseInfo Case<T>(Action<T> action)
        {
            return new CaseInfo
            {
                Action = x => action((T)x),
                Target = typeof(T)
            };
        }

        public static CaseInfo Default(Action action)
        {
            return new CaseInfo
            {
                Action = x => action(),
                IsDefault = true
            };
        }
    }

    internal class TypeToStringConverter : ITypeToStringConverter
    {
        public string GetApiOperationFormat(Type typeToConvert)
        {
            if (IsList(typeToConvert))
                return ListTypeToText(typeToConvert);
            if (IsArray(typeToConvert))
                return ArrayTypeToText(typeToConvert);
            if (IsNullableType(typeToConvert))
                return GetNullableTypeName(typeToConvert);
            return RemoveGenericInfo(typeToConvert);
        }

        public string GetApiOperaiontType(Type typeConvert)
        {
            var ret = "";

            TypeSwitch.Do(
                typeConvert,
                TypeSwitch.Case<ushort>(() => ret = "integer"),
                TypeSwitch.Case<ushort?>(() => ret = "integer"),
                TypeSwitch.Case<short>(() => ret = "integer"),
                TypeSwitch.Case<short?>(() => ret = "integer"),
                TypeSwitch.Case<int>(() => ret = "integer"),
                TypeSwitch.Case<int?>(() => ret = "integer"),
                TypeSwitch.Case<Int32>(() => ret = "integer"),
                TypeSwitch.Case<Int32?>(() => ret = "integer"),
                TypeSwitch.Case<Int64>(() => ret = "integer"),
                TypeSwitch.Case<Int64?>(() => ret = "integer"),
                TypeSwitch.Case<uint>(() => ret = "integer"),
                TypeSwitch.Case<uint?>(() => ret = "integer"),
                TypeSwitch.Case<UInt32>(() => ret = "integer"),
                TypeSwitch.Case<UInt32?>(() => ret = "integer"),
                TypeSwitch.Case<UInt64>(() => ret = "integer"),
                TypeSwitch.Case<UInt64?>(() => ret = "integer"),
                TypeSwitch.Case<long>(() => ret = "integer"),
                TypeSwitch.Case<long?>(() => ret = "integer"),
                TypeSwitch.Case<ulong>(() => ret = "integer"),
                TypeSwitch.Case<ulong?>(() => ret = "integer"),
                TypeSwitch.Case<float>(() => ret = "number"),
                TypeSwitch.Case<float?>(() => ret = "number"),
                TypeSwitch.Case<double>(() => ret = "number"),
                TypeSwitch.Case<double?>(() => ret = "number"),
                TypeSwitch.Case<decimal>(() => ret = "number"),
                TypeSwitch.Case<decimal?>(() => ret = "number"),
                TypeSwitch.Case<string>(() => ret = "string"),
                TypeSwitch.Case<String>(() => ret = "string"),
                TypeSwitch.Case<byte>(() => ret = "string"),
                TypeSwitch.Case<byte?>(() => ret = "string"),
                TypeSwitch.Case<bool>(() => ret = "boolean"),
                TypeSwitch.Case<bool?>(() => ret = "boolean"),
                TypeSwitch.Case<Boolean>(() => ret = "boolean"),
                TypeSwitch.Case<Boolean?>(() => ret = "boolean"),
                TypeSwitch.Case<DateTime>(() => ret = "string"),
                TypeSwitch.Case<DateTime?>(() => ret = "string"),
                TypeSwitch.Default(() => ret = ""));

            return ret;
        }

        private static string GetNullableTypeName(Type typeToConvert)
        {
            return Nullable.GetUnderlyingType(typeToConvert).Name;
        }

        private static bool IsNullableType(Type typeToConvert)
        {
            return Nullable.GetUnderlyingType(typeToConvert) != null;
        }

        private static string ArrayTypeToText(Type returnType)
        {
            return String.Format("array[{0}]", returnType.GetElementType().Name);
        }

        private static bool IsArray(Type returnType)
        {
            return returnType.IsArray;
        }

        private static bool IsList(Type returnType)
        {
            return returnType.IsGenericType &&
                   (returnType.GetGenericTypeDefinition() == typeof(List<>) ||
                    returnType.GetGenericTypeDefinition() == typeof(IList<>) ||
                    returnType.GetGenericTypeDefinition() == typeof(LinkedList<>) ||
                    returnType.GetGenericTypeDefinition() == typeof(Collection<>) ||
                    returnType.GetGenericTypeDefinition() == typeof(ICollection<>) ||
                    returnType.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        }

        private static string ListTypeToText(Type returnType)
        {
            return String.Format("array[{0}]", returnType.GetGenericArguments()[0].Name);
        }

        private static string RemoveGenericInfo(Type type)
        {
            var name = type.Name;
            var index = name.IndexOf('`');
            var ret = index == -1 ? name : name.Substring(0, index);
            return type.Namespace != null && type.Namespace.StartsWith("System") ? ret.ToLower() : ret;
        }
    }
}