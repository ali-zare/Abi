using System;

namespace Abi.Data
{
    public class EntityRelationManagerException : Exception
    {
        public EntityRelationManagerException(string message) : base(message)
        {
        }
        public EntityRelationManagerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class EntityRelationManagerAddException : EntityRelationManagerException
    {
        public EntityRelationManagerAddException(string message) : base(message)
        {
        }
        public EntityRelationManagerAddException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class EntityRelationManagerCanAddException : EntityRelationManagerException
    {
        public EntityRelationManagerCanAddException(string message) : base(message)
        {
        }
        public EntityRelationManagerCanAddException(string message, Exception innerException) : base(message, innerException)
        {
        }
        public EntityRelationManagerCanAddException(string message, byte code) : base(message)
        {
            Data["Code"] = code;
        }
        public EntityRelationManagerCanAddException(string message, Exception innerException, byte code) : base(message, innerException)
        {
            Data["Code"] = code;
        }
    }

    public class EntityRelationManagerRemoveException : EntityRelationManagerException
    {
        public EntityRelationManagerRemoveException(string message) : base(message)
        {
        }
        public EntityRelationManagerRemoveException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class EntityRelationManagerCanRemoveException : EntityRelationManagerException
    {
        public EntityRelationManagerCanRemoveException(string message) : base(message)
        {
        }
        public EntityRelationManagerCanRemoveException(string message, Exception innerException) : base(message, innerException)
        {
        }
        public EntityRelationManagerCanRemoveException(string message, byte code) : base(message)
        {
            Data["Code"] = code;
        }
        public EntityRelationManagerCanRemoveException(string message, Exception innerException, byte code) : base(message, innerException)
        {
            Data["Code"] = code;
        }
    }

    public class EntityRelationManagerIndexerException : EntityRelationManagerException
    {
        public EntityRelationManagerIndexerException(string message) : base(message)
        {
        }
        public EntityRelationManagerIndexerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
