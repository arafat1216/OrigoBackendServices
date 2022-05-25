using System.Collections;

namespace Common.Extensions
{
    internal static class FlatDictionaryExtension
    {
        internal static bool IsValueTypeOrString(this Type type)
        {
            return type.IsValueType || type == typeof(string);
        }

        internal static string ToStringValueType(this object value)
        {
            return value switch
            {
                DateTime dateTime => dateTime.ToString("yyyy-MM-dd"),
                bool boolean => boolean.ToStringLowerCase(),
                _ => value?.ToString()
            };
        }

        internal static bool IsIEnumerable(this Type type)
        {
            return type.IsAssignableTo(typeof(IEnumerable));
        }

        internal static string ToStringLowerCase(this bool boolean)
        {
            return boolean ? "true" : "false";
        }
    }
}
