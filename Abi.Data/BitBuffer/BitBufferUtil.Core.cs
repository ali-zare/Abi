namespace Abi.Data
{
    internal static unsafe partial class BitBufferUtil
    {
        internal static void Set<TBitBuffer>(this ref TBitBuffer Buffer, int Index) where TBitBuffer : unmanaged // C# 7.3 is required
        {
            fixed (TBitBuffer* pbuf = &Buffer)
            {
                byte* p = (byte*)pbuf;
                p += Index / BitPerByte;
                *p |= (byte)(1 << (Index & (BitPerByte - 1)));
            }
        }
        internal static void Reset<TBitBuffer>(this ref TBitBuffer Buffer, int Index) where TBitBuffer : unmanaged // C# 7.3 is required
        {
            fixed (TBitBuffer* pbuf = &Buffer)
            {
                byte* p = (byte*)pbuf;
                p += Index / BitPerByte;
                *p &= (byte)~(1 << (Index & (BitPerByte - 1)));
            }
        }
        internal static bool IsSetted<TBitBuffer>(this ref TBitBuffer Buffer, int Index) where TBitBuffer : unmanaged // C# 7.3 is required
        {
            fixed (TBitBuffer* pbuf = &Buffer)
            {
                byte* p = (byte*)pbuf;
                p += Index / BitPerByte;
                byte b = (byte)(1 << (Index & (BitPerByte - 1)));
                return (*p & b) == b;
            }
        }



        internal static byte[] ToByteArray<TBitBuffer>(this ref TBitBuffer Buffer) where TBitBuffer : unmanaged // C# 7.3 is required
        {
            byte[] ary = new byte[sizeof(TBitBuffer)];

            fixed (byte* p = ary)
            {
                fixed (TBitBuffer* pbuf = &Buffer)
                {
                    *(TBitBuffer*)p = *pbuf;
                }
            }

            return ary;
        }



        internal static void CopyTo<TSourceBitBuffer, TDestinationBitBuffer>(this ref TSourceBitBuffer SourceBuffer, ref TDestinationBitBuffer DestinationBuffer, int BitIndex = 0) where TSourceBitBuffer : unmanaged where TDestinationBitBuffer : unmanaged // C# 7.3 is required
        {
            int SrcBufBitSize = BitPerByte * sizeof(TSourceBitBuffer);
            int DisBufBitSize = BitPerByte * sizeof(TDestinationBitBuffer);

            if (DisBufBitSize < SrcBufBitSize + BitIndex)
                throw new BitBufferException($"Can Not Copy {typeof(TSourceBitBuffer).Name} To {typeof(TDestinationBitBuffer).Name}, Because Of Source Is Smaller Than Destination");


            if (SrcBufBitSize < BitPerShort)
                SourceBuffer.Copy1(ref DestinationBuffer, BitIndex);

            else if (SrcBufBitSize < BitPerInt)
                SourceBuffer.Copy2(ref DestinationBuffer, BitIndex);

            else if (SrcBufBitSize < BitPerLong)
                SourceBuffer.Copy4(ref DestinationBuffer, BitIndex);

            else
                SourceBuffer.Copy8(ref DestinationBuffer, BitIndex);
        }

        private static void Copy1<TSourceBitBuffer, TDestinationBitBuffer>(this ref TSourceBitBuffer SourceBuffer, ref TDestinationBitBuffer DestinationBuffer, int BitIndex, int SrcBufByteOffset = 0) where TSourceBitBuffer : unmanaged where TDestinationBitBuffer : unmanaged // C# 7.3 is required
        {
            int SrcBufSize = sizeof(TSourceBitBuffer);
            int SrcBufByteCount = SrcBufSize;

            int SplitBitIndex = BitIndex & (BitPerByte - 1);

            fixed (TSourceBitBuffer* pSrcBuf = &SourceBuffer)
            {
                byte* sp = (byte*)pSrcBuf;
                sp += SrcBufByteOffset;

                fixed (TDestinationBitBuffer* pDisBuf = &DestinationBuffer)
                {
                    byte* dp = (byte*)pDisBuf;
                    dp += (BitIndex / BitPerByte); // move to first byte that must be changed.

                    for (int SrcBufByteIndex = SrcBufByteOffset; SrcBufByteIndex < SrcBufByteCount; SrcBufByteIndex++)
                    {
                        *dp &= (byte)(ByteMask >> (BitPerByte - SplitBitIndex));
                        *dp |= (byte)(*sp << (SplitBitIndex)); // lower bits of byte : *sp

                        dp += 1;

                        *dp &= (byte)(ByteMask << (SplitBitIndex)); // replaced with old code : // *dp <<= SplitBitIndex;
                        *dp |= (byte)(*sp >> (BitPerByte - SplitBitIndex)); // upper bits of byte : *sp

                        sp += 1;
                    }
                }
            }
        }
        private static void Copy2<TSourceBitBuffer, TDestinationBitBuffer>(this ref TSourceBitBuffer SourceBuffer, ref TDestinationBitBuffer DestinationBuffer, int BitIndex, int SrcBufByteOffset = 0) where TSourceBitBuffer : unmanaged where TDestinationBitBuffer : unmanaged // C# 7.3 is required
        {
            int SrcBufSize = sizeof(TSourceBitBuffer);
            int SrcBufShortCount = SrcBufSize / BytePerShort;

            int SrcBufShortOffset = SrcBufByteOffset / BytePerShort;
            int SplitBitIndex = BitIndex & (BitPerShort - 1);

            if (SrcBufShortOffset == SrcBufShortCount)
                SourceBuffer.Copy1(ref DestinationBuffer, BitIndex, SrcBufByteOffset);
            else
            {
                fixed (TSourceBitBuffer* pSrcBuf = &SourceBuffer)
                {
                    ushort* sp = (ushort*)pSrcBuf;
                    sp += SrcBufShortOffset;

                    fixed (TDestinationBitBuffer* pDisBuf = &DestinationBuffer)
                    {
                        ushort* dp = (ushort*)pDisBuf;
                        dp += BitIndex / BitPerShort; // move to first uint that must be changed.

                        for (int SrcBufShortIndex = SrcBufShortOffset; SrcBufShortIndex < SrcBufShortCount; SrcBufShortIndex++)
                        {
                            *dp &= (ushort)(ShortMask >> (BitPerShort - SplitBitIndex));
                            *dp |= (ushort)(*sp << (SplitBitIndex)); // lower bits of uint : *sp

                            dp += 1;

                            *dp &= (ushort)(ShortMask << (SplitBitIndex));
                            *dp |= (ushort)(*sp >> (BitPerShort - SplitBitIndex)); // upper bits of uint : *sp

                            sp += 1;
                        }
                    }
                }

                if ((SrcBufSize & (BytePerShort - 1)) > 0)
                    SourceBuffer.Copy1(ref DestinationBuffer, BitIndex + ((SrcBufShortCount - SrcBufShortOffset) * BitPerShort), SrcBufShortCount * BytePerShort);
            }
        }
        private static void Copy4<TSourceBitBuffer, TDestinationBitBuffer>(this ref TSourceBitBuffer SourceBuffer, ref TDestinationBitBuffer DestinationBuffer, int BitIndex, int SrcBufByteOffset = 0) where TSourceBitBuffer : unmanaged where TDestinationBitBuffer : unmanaged // C# 7.3 is required
        {
            int SrcBufSize = sizeof(TSourceBitBuffer);
            int SrcBufIntCount = SrcBufSize / BytePerInt;

            int SrcBufIntOffset = SrcBufByteOffset / BytePerInt;
            int SplitBitIndex = BitIndex & (BitPerInt - 1);

            if (SrcBufIntOffset == SrcBufIntCount)
                SourceBuffer.Copy2(ref DestinationBuffer, BitIndex, SrcBufByteOffset);
            else
            {
                fixed (TSourceBitBuffer* pSrcBuf = &SourceBuffer)
                {
                    uint* sp = (uint*)pSrcBuf;
                    sp += SrcBufIntOffset;

                    fixed (TDestinationBitBuffer* pDisBuf = &DestinationBuffer)
                    {
                        uint* dp = (uint*)pDisBuf;
                        dp += BitIndex / BitPerInt; // move to first uint that must be changed.

                        for (int SrcBufIntIndex = SrcBufIntOffset; SrcBufIntIndex < SrcBufIntCount; SrcBufIntIndex++)
                        {
                            *dp &= IntMask >> (BitPerInt - SplitBitIndex);
                            *dp |= *sp << (SplitBitIndex); // lower bits of uint : *sp

                            dp += 1;

                            *dp &= IntMask << (SplitBitIndex);
                            *dp |= *sp >> (BitPerInt - SplitBitIndex); // upper bits of uint : *sp

                            sp += 1;
                        }
                    }
                }

                if ((SrcBufSize & (BytePerInt - 1)) > 0)
                    SourceBuffer.Copy2(ref DestinationBuffer, BitIndex + ((SrcBufIntCount - SrcBufIntOffset) * BitPerInt), SrcBufIntCount * BytePerInt);
            }
        }
        private static void Copy8<TSourceBitBuffer, TDestinationBitBuffer>(this ref TSourceBitBuffer SourceBuffer, ref TDestinationBitBuffer DestinationBuffer, int BitIndex) where TSourceBitBuffer : unmanaged where TDestinationBitBuffer : unmanaged // C# 7.3 is required
        {
            int SrcBufSize = sizeof(TSourceBitBuffer);
            int SrcBufLongCount = SrcBufSize / BytePerLong;

            int SplitBitIndex = BitIndex & (BitPerLong - 1);

            fixed (TSourceBitBuffer* pSrcBuf = &SourceBuffer)
            {
                ulong* sp = (ulong*)pSrcBuf;

                fixed (TDestinationBitBuffer* pDisBuf = &DestinationBuffer)
                {
                    ulong* dp = (ulong*)pDisBuf;
                    dp += BitIndex / BitPerLong; // move to first ulong that must be changed.

                    for (int SrcBufLongIndex = 0; SrcBufLongIndex < SrcBufLongCount; SrcBufLongIndex++)
                    {
                        *dp &= LongMask >> (BitPerLong - SplitBitIndex);
                        *dp |= *sp << (SplitBitIndex); // lower bits of ulong : *sp

                        dp += 1;

                        *dp &= LongMask << (SplitBitIndex);
                        *dp |= *sp >> (BitPerLong - SplitBitIndex); // upper bits of ulong : *sp

                        sp += 1;
                    }
                }
            }

            if ((SrcBufSize & (BytePerLong - 1)) > 0)
                SourceBuffer.Copy4(ref DestinationBuffer, BitIndex + (SrcBufLongCount * BitPerLong), SrcBufLongCount * BytePerLong);
        }
    }
}
