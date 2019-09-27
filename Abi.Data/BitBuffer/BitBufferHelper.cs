using System;

namespace Abi.Data
{
    internal static class BitBufferHelper
    {
        internal static Type GetBitBufferType(int Size)
        {
            if (Size <= 0)
                throw new BitBufferException($"GetBitBuffer Failed, Size Is Out Of Range. Can Not Get BitBuffer For Size {Size}");

            if (Size <= 8)
                return typeof(BitBuffer1);

            else if (Size <= 16)
                return typeof(BitBuffer2);

            else if (Size <= 24)
                return typeof(BitBuffer3);

            else if (Size <= 32)
                return typeof(BitBuffer4);

            else if (Size <= 48)
                return typeof(BitBuffer6);

            else if (Size <= 64)
                return typeof(BitBuffer8);

            else if (Size <= 96)
                return typeof(BitBuffer12);

            else if (Size <= 128)
                return typeof(BitBuffer16);

            else if (Size <= 192)
                return typeof(BitBuffer24);

            else if (Size <= 256)
                return typeof(BitBuffer32);

            else
                throw new BitBufferException($"GetBitBuffer Failed, Size Over 256 Is Not Supported, Can Not Found Appropriate Bit Buffer For Size {Size}");

        }

    }
}
