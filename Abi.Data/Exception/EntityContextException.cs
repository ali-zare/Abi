using System;

namespace Abi.Data
{
    public class EntityContextException : Exception
    {
        public EntityContextException(string message) : base(message)
        {
        }
        public EntityContextException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class EntityContextBusyException : Exception
    {
        public EntityContextBusyException(string message) : base(message)
        {
        }
        public EntityContextBusyException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
