using System;

namespace Abi.Test
{
    public class NotExpectedResultException : Exception
    {
        public NotExpectedResultException(string message) : base(message)
        {
        }
        public NotExpectedResultException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
