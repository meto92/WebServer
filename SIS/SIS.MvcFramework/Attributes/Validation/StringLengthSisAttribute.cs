namespace SIS.MvcFramework.Attributes.Validation
{
    public class StringLengthSisAttribute : ValidationSisAttribute
    {
        private const string StringLengthErrorMessage = "Length must be between {0} and {1}.";

        private readonly int minLength;
        private readonly int maxLength;

        public StringLengthSisAttribute(int minLength, int maxLength) 
            : base()
        {
            this.minLength = minLength;
            this.maxLength = maxLength;
        }

        public override string ErrorMessage
            => string.Format(StringLengthErrorMessage, this.minLength, this.maxLength);

        public override bool IsValid(object value)
        {
            string stringValue = value.ToString();

            return stringValue.Length >= this.minLength 
                && stringValue.Length <= this.maxLength;
        }
    }
}