using System.Runtime.InteropServices;

namespace Abi.Data
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    internal partial struct BitBuffer15
    {
        [FieldOffset(0)] private ulong s1;
        [FieldOffset(8)] private uint s2;
        [FieldOffset(12)] private ushort s3;
        [FieldOffset(14)] private byte s4;

        public bool IsSet()
        {
            return s1 > 0 || s2 > 0 || s3 > 0 || s4 > 0;
        }
        public void SetAll()
        {
            s1 = BitBufferUtil.LongMask;
            s2 = BitBufferUtil.IntMask;
            s3 = BitBufferUtil.ShortMask;
            s4 = BitBufferUtil.ByteMask;
        }
        public void Clear()
        {
            s1 = s2 = s3 = s4 = 0;
        }

        public override string ToString()
        {
            return this.ToByteArray().ToBinaryString();
        }
    }
}
