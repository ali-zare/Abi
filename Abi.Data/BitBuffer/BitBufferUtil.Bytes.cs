using System;
using System.Linq;
using System.Collections.Generic;

namespace Abi.Data
{
    internal static unsafe partial class BitBufferUtil
    {
        public static IEnumerable<T> GetSetted<T>(this byte[] BufferBytes, T[] Source)
        {
            int Size = BufferBytes.Length;

            int byteCount = Source.Length / BitPerByte;
            int lastByteBitCount = Source.Length & (BitPerByte - 1);
            int sourceSize = byteCount + (lastByteBitCount > 0 ? 1 : 0);

            if (sourceSize > Size)
                throw new BitBufferException($"{typeof(BitBuffer15).Name} GetSetted Item Failed, Source Size ({sourceSize}) Can Not Be Grater Than BitBuffer Size ({Size}) ");

            for (int byteIndex = 0; byteIndex < byteCount; byteIndex++)
            {
                int offset = byteIndex * BitPerByte;
                byte b = BufferBytes[byteIndex];

                byte bitIndex = 0;
                do
                {
                    if ((b & (1 << bitIndex)) > 0)
                        yield return Source[offset + bitIndex];
                } while (++bitIndex < BitPerByte);
            }

            if (sourceSize > byteCount)
            {
                int offset = byteCount * BitPerByte;
                byte b = BufferBytes[byteCount];

                byte bitIndex = 0;
                do
                {
                    if ((b & (1 << bitIndex)) > 0)
                        yield return Source[offset + bitIndex];
                } while (++bitIndex < lastByteBitCount);
            }
        }

        public static string ToBinaryString(this byte[] BufferBytes)
        {
            return string.Join(" ", BufferBytes.Reverse().Select(x => Convert.ToString(x, 2).PadLeft(8, '0')));
        }

   }
}
