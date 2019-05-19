using SIS.HTTP.Common;

namespace SIS.HTTP.Extensions
{
    public static class StringExtensions
    {
        public static string Capitalize(this string strToCapitalize)
        {
            CoreValidator.ThrowIfNullOrEmpty(strToCapitalize, nameof(strToCapitalize));
            
            return char.ToUpper(strToCapitalize[0]) +
                strToCapitalize.Substring(1, strToCapitalize.Length - 1).ToLower();
        }
    }
}