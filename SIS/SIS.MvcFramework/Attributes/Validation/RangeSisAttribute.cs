using System;
using System.Linq;

namespace SIS.MvcFramework.Attributes.Validation
{
    public class RangeSisAttribute : ValidationSisAttribute
    {
        private const string RangeErrorMessage = "Value must be in range [{0}, {1}].";

        private readonly object minValue;
        private readonly object maxValue;
        private readonly Type objectType;
        private readonly Type[] supportedTypes;

        private RangeSisAttribute(Type objectType, object minValue, object maxValue)
        {
            this.objectType = objectType;
            this.minValue = minValue;
            this.maxValue = maxValue;

            this.supportedTypes = new[]
            {
                typeof(int),
                typeof(double),
                typeof(decimal)
            };
        }

        public RangeSisAttribute(int minValue, int maxValue)
            : this(typeof(int), minValue, maxValue)
        { }

        public RangeSisAttribute(double minValue, double maxValue)
            : this(typeof(double), minValue, maxValue)
        { }

        public RangeSisAttribute(Type type, string minValue, string maxValue)
            : this(type, Convert.ChangeType(minValue, type), Convert.ChangeType(maxValue, type))
        { }

        public override string ErrorMessage
            => string.Format(RangeErrorMessage, this.minValue, this.maxValue);

        public override bool IsValid(object value)
        {
            if (!this.supportedTypes.Contains(this.objectType))
            {
                throw new NotImplementedException();
            }
            
            try
            {
                decimal decimalValue = (decimal) Convert.ChangeType(value, typeof(decimal));
                decimal decimalMinValue = (decimal) Convert.ChangeType(this.minValue, typeof(decimal));
                decimal decimalMaxValue = (decimal) Convert.ChangeType(this.maxValue, typeof(decimal));

                return decimalValue >= decimalMinValue
                    && decimalValue <= decimalMaxValue;
            }
            catch
            {
                return false;
            }
        }
    }
}