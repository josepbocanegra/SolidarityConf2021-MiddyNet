using System;
using System.Runtime.Serialization;

namespace WithMiddyNet
{
    [Serializable]
    internal class WrongGreetingTypeException : Exception
    {
        public WrongGreetingTypeException()
        {
        }

        public WrongGreetingTypeException(string message) : base(message)
        {
        }

        public WrongGreetingTypeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WrongGreetingTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}