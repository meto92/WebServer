using System.Text.RegularExpressions;

namespace SIS.MvcFramework.Attributes.Validation
{
    public class RegexSisAttribute : ValidationSisAttribute
    {
        private const string RegexErrorMessage = "Invalid value.";

        private readonly Regex regex;

        public RegexSisAttribute(string regexStr)
        {
            this.regex = new Regex($"^{regexStr}$");
        }

        public override string ErrorMessage => RegexErrorMessage;

        public override bool IsValid(object value)
            => this.regex.IsMatch(value.ToString());
    }
}