namespace Abi.Types
{
    public struct RowVersion
    {
        #region .Ctr

        public RowVersion(ulong Value)
        {
            value = Value;
        }

        private ulong value;

        #endregion .Ctr

        #region Parse & ToString

        public override string ToString()
        {
            return value.ToString();
        }

        #endregion Parse & ToString

        #region Operator

        public static implicit operator ulong(RowVersion rv)
        {
            return rv.value;
        }
        public static implicit operator RowVersion(ulong v)
        {
            return new RowVersion(v);
        }

        public static implicit operator ulong?(RowVersion? rv)
        {
            if (rv == null)
                return null;
            else
                return rv.Value.value;
        }
        public static implicit operator RowVersion?(ulong? v)
        {
            if (v == null)
                return null;
            else
                return new RowVersion(v.Value);
        }


        public static bool operator ==(RowVersion rv1, RowVersion rv2)
        {
            return rv1.value == rv2.value;
        }
        public static bool operator !=(RowVersion rv1, RowVersion rv2)
        {
            return !(rv1 == rv2);
        }

        #endregion Operator

        #region Equals & GetHashCode

        public override bool Equals(object obj)
        {
            if (obj is RowVersion rowVersion)
                return rowVersion.value == value;

            return false;
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        #endregion Equals & GetHashCode
    }
}
