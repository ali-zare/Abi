using System.Runtime.InteropServices;

namespace Abi.Data
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct BitBuffer24
    {
        [FieldOffset(0)] private ulong s1;
        [FieldOffset(8)] private ulong s2;
        [FieldOffset(16)] private ulong s3;

        public bool IsSet()
        {
            return s1 > 0 || s2 > 0 || s3 > 0;
        }
        public void SetAll()
        {
            s1 = BitBufferUtil.LongMask;
            s2 = BitBufferUtil.LongMask;
            s3 = BitBufferUtil.LongMask;
        }
        public void Clear()
        {
            s1 = s2 = s3 = 0;
        }

        public override string ToString()
        {
            return this.ToByteArray().ToBinaryString();
        }
    }
}
