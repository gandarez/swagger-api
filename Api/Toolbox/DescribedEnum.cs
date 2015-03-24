using System.ComponentModel;

namespace Swagger.Api.Toolbox
{
    public struct DescribedEnum<T> where T : struct
    {
        private T _value;
        public DescribedEnum(T value) { _value = value; }
       
        public override string ToString()
        {
            try
            {
                var text = _value.ToString();
                var attr = typeof(T).GetField(text).GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attr.Length == 1)                
                    text = ((DescriptionAttribute)attr[0]).Description;
                
                return text;
            }
            catch
            {
                return string.Empty;
            }
        }
        
        public static implicit operator DescribedEnum<T>(T value)
        {
            return new DescribedEnum<T>(value);
        }

        public static implicit operator T(DescribedEnum<T> value)
        {
            return value._value;
        }
    }
}