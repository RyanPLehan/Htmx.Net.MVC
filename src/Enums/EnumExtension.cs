using System.ComponentModel;

namespace ContosoUniversity.Enums
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Methods were taken from 
    /// https://medium.com/@hafiz.arslanelahi/cleaning-enum-code-in-c-with-enum-extension-methods-and-reflection-85c99419926f
    /// </remarks>
    public static class EnumExtension
    {
        /// <summary>
        /// Retrieve a specific attribute of type T from an enum value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(this Enum value) where T : Attribute
        {
            var type = value.GetType();
            var memberInfo = type.GetMember(value.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
            return attributes.Length > 0 ? (T)attributes[0] : null;
        }


        /// <summary>
        /// Get the description of an enum value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum value)
        {
            var attribute = value.GetAttribute<DescriptionAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }


        /// <summary>
        /// Get the underlying type of the enum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static Type GetUnderlyingType<T>(this T enumValue) where T : Enum
        {
            return Enum.GetUnderlyingType(typeof(T));
        }

        
        /// <summary>
        /// Parse a string to get an enum value 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Parse<T>(this string value) where T : Enum
        {
            return (T)Enum.Parse(typeof(T), value);
        }

        
        /// <summary>
        /// Get the name of the enum value 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetName<T>(this T enumValue) where T : Enum
        {
            return Enum.GetName(typeof(T), enumValue);
        }

        
        /// <summary>
        /// Compare the enum value with another 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumValue"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static int CompareTo<T>(this T enumValue, T other) where T : Enum
        {
            return enumValue.CompareTo(other);
        }


        /// <summary>
        /// Check if the enum values are equal
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumValue"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool Equals<T>(this T enumValue, T other) where T : Enum
        {
            return enumValue.Equals(other);
        }

        
        /// <summary>
        /// Get the hash code for the enum value 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static int GetHashCode<T>(this T enumValue) where T : Enum
        {
            return enumValue.GetHashCode();
        }


        /// <summary>
        /// Check if the enum value contains a flag 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumValue"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static bool HasFlag<T>(this T enumValue, T flag) where T : Enum
        {
            return enumValue.HasFlag(flag);
        }


        /// <summary>
        /// Retrieve all enum values as a list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> GetValues<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }

        /// <summary>
        /// Check if an enum value is defined in the enum type 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static bool IsDefined<T>(this T enumValue) where T : Enum
        {
            return Enum.IsDefined(typeof(T), enumValue);
        }
    }
}
