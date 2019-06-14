using System.Text.RegularExpressions;

namespace SIS.MvcFramework.Attributes.Validation
{
    public class EmailSisAttribute : ValidationSisAttribute
    {
        private const string EmailErrorMessage = "Invalid email address.";

        public override string ErrorMessage => EmailErrorMessage;

        public override bool IsValid(object value)
        {
            string stringValue = value.ToString();

            return Regex.IsMatch(stringValue, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
        }
    }
}