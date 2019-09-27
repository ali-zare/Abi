using System;

namespace Abi.Data.Sql
{
    public class EntityDatabaseConcurrencyException : Exception
    {
        public EntityDatabaseConcurrencyException(string message) : base(message)
        {
        }
        public EntityDatabaseConcurrencyException(string message, Exception innerException) : base(message, innerException)
        {
        }
        public EntityDatabaseConcurrencyException(string message, params Entity[] entities) : base(message)
        {
            Entities = entities;
        }
        public EntityDatabaseConcurrencyException(string message, Exception innerException, params Entity[] entities) : base(message, innerException)
        {
            Entities = entities;
        }

        public Entity[] Entities { get; }
    }
}
