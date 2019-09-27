using System;

namespace Abi.Data
{
    public class EntityException : Exception
    {
        public EntityException(string message) : base(message)
        {
        }
        public EntityException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class EntityEditException : EntityException
    {
        public EntityEditException(string message) : base(message)
        {
        }
        public EntityEditException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class EntityPropertySetException : EntityException
    {
        public EntityPropertySetException(string message) : base(message)
        {
        }
        public EntityPropertySetException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class EntitySnapshotException : EntityException
    {
        public EntitySnapshotException(string message) : base(message)
        {
        }
        public EntitySnapshotException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

}
