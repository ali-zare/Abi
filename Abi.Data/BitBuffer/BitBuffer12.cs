using System.Runtime.InteropServices;

namespace Abi.Data
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct BitBuffer12
    {
        [FieldOffset(0)] private uint s1;
        [FieldOffset(4)] private uint s2;
        [FieldOffset(8)] private uint s3;

        public bool IsSet()
        {
            return s1 > 0 || s2 > 0 || s3 > 0;
        }
        public void SetAll()
        {
            s1 = BitBufferUtil.IntMask;
            s2 = BitBufferUtil.IntMask;
            s3 = BitBufferUtil.IntMask;
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
