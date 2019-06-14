namespace SIS.MvcFramework.Attributes.Validation
{
    public class PasswordSisAttribute : ValidationSisAttribute
    {
        public const string PasswordsDoNotMatchMessage = "Passwords do not match.";
        public const string ConfirmPasswordPropertyNotFound = "Confirm password property not found.";

        private const string PasswordErrorMessage = "Password's minimum length is {0}";

        private readonly int minLength;

        public PasswordSisAttribute(int minLength, string confirmPasswordPropertyName)
        {
            this.minLength = minLength;
            this.ConfirmPasswordPropertyName = confirmPasswordPropertyName;
        }

        public string ConfirmPasswordPropertyName { get; }

        public override string ErrorMessage
            => string.Format(PasswordErrorMessage, this.minLength);

        public override bool IsValid(object value)
            => value.ToString().Length >= this.minLength;
    }
}