using System;
using System.Runtime.Serialization;

namespace Maths_Software_with_Interpreter
{
    [Serializable]
    // InvalidTokenException is thrown when the lexer comes across an invalid token in the interpreter input
    internal class InvalidTokenException : Exception
    {
        public InvalidTokenException()
        {
        }

        public InvalidTokenException(string message) : base(message)
        {
        }

        public InvalidTokenException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidTokenException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}