namespace Abi.Data.Sql
{
    internal static unsafe partial class BufferUtil
    {
        internal static ulong RowVersionToUInt64(this byte[] Buffer)
        {
            byte[] buffer = new byte[] { Buffer[7], Buffer[6], Buffer[5], Buffer[4], Buffer[3], Buffer[2], Buffer[1], Buffer[0] };

            ulong l;

            fixed (byte* pbuf = buffer)
            {
                l = *(ulong*)pbuf;
            }

            return l;
        }

        internal static byte[] GetBytes(this ulong l)
        {
            byte[] buffer = new byte[8];

            fixed (byte* pb = buffer)
            {
                *(ulong*)pb = l;
            }

            return new byte[] { buffer[7], buffer[6], buffer[5], buffer[4], buffer[3], buffer[2], buffer[1], buffer[0] };
        }

    }
}
