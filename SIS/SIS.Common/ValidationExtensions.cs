using System;

namespace SIS.Common
{
    public static class ValidationExtensions
    {
        private const string InvalidStringMessage = "{0} cannot be null or empty.";

        public static void ThrowIfNull(this object obj, string name)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        public static void ThrowIfNullOrEmpty(this string str, string name)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentException(
                    string.Format(InvalidStringMessage, name));
            }
        }
    }
}