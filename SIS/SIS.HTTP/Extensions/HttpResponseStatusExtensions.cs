using System.Text.RegularExpressions;

using SIS.HTTP.Enums;

namespace SIS.HTTP.Extensions
{
    public static class HttpResponseStatusExtensions
    {
        public static string GetResponseLine(this HttpResponseStatusCode httpResponseStatusCode)
        {
            string result = $"{(int) httpResponseStatusCode} ";

            result += string.Join(" ", Regex.Matches(
                httpResponseStatusCode.ToString(),
                "[A-Z]+[a-z]*"));

            return result;
        }
    }
}