using System;
using System.Runtime.Serialization;

namespace Maths_Software_with_Interpreter
{
    [Serializable]
    // UnknownFunctionException is thrown when an unknown function is found
    internal class UnknownFunctionException : Exception
    {
        public UnknownFunctionException()
        {
        }

        public UnknownFunctionException(string message) : base(message)
        {
        }

        public UnknownFunctionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnknownFunctionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}