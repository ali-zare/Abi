using System;

namespace Abi.Data
{
    public class EntitySetException : Exception
    {
        public EntitySetException(string message) : base(message)
        {
        }
        public EntitySetException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }


    public class EntitySetAcceptEntityException : EntitySetException
    {
        public EntitySetAcceptEntityException(string message) : base(message)
        {
        }
        public EntitySetAcceptEntityException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }


    public class EntitySetAddException : EntitySetException
    {
        public EntitySetAddException(string message) : base(message)
        {
        }
        public EntitySetAddException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
    public class EntitySetCanAddException : EntitySetException
    {
        public EntitySetCanAddException(string message) : base(message)
        {
        }
        public EntitySetCanAddException(string message, Exception innerException) : base(message, innerException)
        {
        }
        public EntitySetCanAddException(string message, byte code) : base(message)
        {
            Data["Code"] = code;
        }
        public EntitySetCanAddException(string message, Exception innerException, byte code) : base(message, innerException)
        {
            Data["Code"] = code;
        }
    }


    public class EntitySetRemoveException : EntitySetException
    {
        public EntitySetRemoveException(string message) : base(message)
        {
        }
        public EntitySetRemoveException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
    public class EntitySetCanRemoveException : EntitySetException
    {
        public EntitySetCanRemoveException(string message) : base(message)
        {
        }
        public EntitySetCanRemoveException(string message, Exception innerException) : base(message, innerException)
        {
        }
        public EntitySetCanRemoveException(string message, byte code) : base(message)
        {
            Data["Code"] = code;
        }
        public EntitySetCanRemoveException(string message, Exception innerException, byte code) : base(message, innerException)
        {
            Data["Code"] = code;
        }
    }

    
    public class EntitySetEditException : EntitySetException
    {
        public EntitySetEditException(string message) : base(message)
        {
        }
        public EntitySetEditException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
    public class EntitySetCanEditException : EntitySetException
    {
        public EntitySetCanEditException(string message) : base(message)
        {
        }
        public EntitySetCanEditException(string message, Exception innerException) : base(message, innerException)
        {
        }
        public EntitySetCanEditException(string message, byte code) : base(message)
        {
            Data["Code"] = code;
        }
        public EntitySetCanEditException(string message, Exception innerException, byte code) : base(message, innerException)
        {
            Data["Code"] = code;
        }
    }


    public class EntitySetDetachException : EntitySetException
    {
        public EntitySetDetachException(string message) : base(message)
        {
        }
        public EntitySetDetachException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
    public class EntitySetCanDetachException : EntitySetException
    {
        public EntitySetCanDetachException(string message) : base(message)
        {
        }
        public EntitySetCanDetachException(string message, Exception innerException) : base(message, innerException)
        {
        }
        public EntitySetCanDetachException(string message, byte code) : base(message)
        {
            Data["Code"] = code;
        }
        public EntitySetCanDetachException(string message, Exception innerException, byte code) : base(message, innerException)
        {
            Data["Code"] = code;
        }
    }


    public class EntitySetIndexerException : EntitySetException
    {
        public EntitySetIndexerException(string message) : base(message)
        {
        }
        public EntitySetIndexerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }


    public class EntitySetContainsException : EntitySetException
    {
        public EntitySetContainsException(string message) : base(message)
        {
        }
        public EntitySetContainsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }


    public class EntitySetInsertException : EntitySetException
    {
        public EntitySetInsertException(string message) : base(message)
        {
        }
        public EntitySetInsertException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }


    public class EntitySetRemoveAtException : EntitySetException
    {
        public EntitySetRemoveAtException(string message) : base(message)
        {
        }
        public EntitySetRemoveAtException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }


    public class EntitySetClearException : EntitySetException
    {
        public EntitySetClearException(string message) : base(message)
        {
        }
        public EntitySetClearException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }


    public class EntitySetLateAddException : EntitySetException
    {
        public EntitySetLateAddException(string message) : base(message)
        {
        }
        public EntitySetLateAddException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
