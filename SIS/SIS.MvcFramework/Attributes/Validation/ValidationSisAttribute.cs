using System;

namespace SIS.MvcFramework.Attributes.Validation
{
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class ValidationSisAttribute : Attribute
    {
        public abstract string ErrorMessage { get; }

        public abstract bool IsValid(object value);
    }
}