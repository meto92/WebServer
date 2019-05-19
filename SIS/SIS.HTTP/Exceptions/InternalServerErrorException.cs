using System;

namespace SIS.HTTP.Exceptions
{
    public class InternalServerErrorException : Exception
    {
        private const string DefaultMessage = "The Server has encountered an error.";

        public InternalServerErrorException(string message = DefaultMessage)
            : base(message)
        { }
    }
}