using System;
using System.Runtime.Serialization;

namespace Maths_Software_with_Interpreter
{
    [Serializable]
    // NonVariableException is thrown when a number/variable is assigned to a non-variable/e/π
    internal class NonVariableException : Exception
    {
        public NonVariableException()
        {
        }

        public NonVariableException(string message) : base(message)
        {
        }

        public NonVariableException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NonVariableException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}