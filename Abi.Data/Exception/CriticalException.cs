using System;

namespace Abi.Data
{
    public class CriticalException : Exception
    {
        public CriticalException(string message) : base(message)
        {
        }
        public CriticalException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
