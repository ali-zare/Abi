using System;

namespace Abi.Data.Sql
{
    public class EntityDatabaseException : Exception
    {
        public EntityDatabaseException(string message) : base(message)
        {
        }
        public EntityDatabaseException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
