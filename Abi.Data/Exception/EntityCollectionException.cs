using System;

namespace Abi.Data
{
    public class EntityCollectionException : Exception
    {
        public EntityCollectionException(string message) : base(message)
        {
        }
        public EntityCollectionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class EntityCollectionInitializeException : EntityCollectionException
    {
        public EntityCollectionInitializeException(string message) : base(message)
        {
        }
        public EntityCollectionInitializeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class EntityCollectionIndexerException : EntityCollectionException
    {
        public EntityCollectionIndexerException(string message) : base(message)
        {
        }
        public EntityCollectionIndexerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class EntityCollectionInsertException : EntityCollectionException
    {
        public EntityCollectionInsertException(string message) : base(message)
        {
        }
        public EntityCollectionInsertException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class EntityCollectionRemoveAtException : EntityCollectionException
    {
        public EntityCollectionRemoveAtException(string message) : base(message)
        {
        }
        public EntityCollectionRemoveAtException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class EntityCollectionRemoveException : EntityCollectionException
    {
        public EntityCollectionRemoveException(string message) : base(message)
        {
        }
        public EntityCollectionRemoveException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
