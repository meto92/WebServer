using System;

namespace SIS.HTTP.Common
{
    public class CoreValidator
    {
        private const string InvalidStringMessage = "{0} cannot be null or empty.";

        public static void ThrowIfNull(object obj, string name)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        public static void ThrowIfNullOrEmpty(string str, string name)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentException(
                    string.Format(InvalidStringMessage, name));
            }
        }
    }
}