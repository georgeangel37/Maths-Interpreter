using System;
using System.Runtime.Serialization;

namespace Maths_Software_with_Interpreter
{
    [Serializable]
    // InvalidSyntaxException is thrown when the parser comes across invalid syntax from the series of input tokens
    internal class InvalidSyntaxException : Exception
    {
        public InvalidSyntaxException()
        {
        }

        public InvalidSyntaxException(string message) : base(message)
        {
        }

        public InvalidSyntaxException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidSyntaxException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}