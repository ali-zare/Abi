using System;

namespace Abi.Data
{
    public class EntityKeyManagerException : Exception
    {
        public EntityKeyManagerException(string message) : base(message)
        {
        }
        public EntityKeyManagerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
