using System;

namespace Abi.Data
{
    public class EntityContextConfigurationException : Exception
    {
        public EntityContextConfigurationException(string message) : base(message)
        {
        }
        public EntityContextConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
