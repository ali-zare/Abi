namespace Abi.Data
{
    internal static partial class BitBufferUtil
    {
        private const int BytePerShort = 2;
        private const int BytePerInt = 4;
        private const int BytePerLong = 8;

        private const int BitPerByte = 8;
        private const int BitPerShort = 16;
        private const int BitPerInt = 32;
        private const int BitPerLong = 64;

        internal const byte ByteMask = 0xFF;
        internal const ushort ShortMask = 0xFFFF;
        internal const uint IntMask = 0xFFFFFFFF;
        internal const ulong LongMask = 0xFFFFFFFFFFFFFFFF;
    }
}
