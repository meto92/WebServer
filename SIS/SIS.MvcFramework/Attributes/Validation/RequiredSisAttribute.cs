namespace SIS.MvcFramework.Attributes.Validation
{
    public class RequiredSisAttribute : ValidationSisAttribute
    {
        private const string RequiredErrorMessage = "Required.";

        public override string ErrorMessage => RequiredErrorMessage;

        public override bool IsValid(object value)
            => value != null;
    }
}