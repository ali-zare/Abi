using System;

namespace Abi.Data
{
    public class EntityUniqueException : Exception
    {
        public EntityUniqueException(string message) : base(message)
        {
        }
        public EntityUniqueException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class EntityUniqueInitializeException : EntityUniqueException
    {
        public EntityUniqueInitializeException(string message) : base(message)
        {
        }
        public EntityUniqueInitializeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class EntityUniqueSetException : EntityUniqueException
    {
        public EntityUniqueSetException(string message) : base(message)
        {
        }
        public EntityUniqueSetException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
