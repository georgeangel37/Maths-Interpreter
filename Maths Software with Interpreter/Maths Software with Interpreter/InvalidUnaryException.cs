using System;
using System.Runtime.Serialization;

namespace Maths_Software_with_Interpreter
{
    [Serializable]
    // InvalidUnaryException is thrown when the parser comes across invalid usage of unary operators
    internal class InvalidUnaryException : Exception
    {
        public InvalidUnaryException()
        {
        }

        public InvalidUnaryException(string message) : base(message)
        {
        }

        public InvalidUnaryException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidUnaryException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
