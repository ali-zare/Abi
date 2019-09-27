using System.Runtime.InteropServices;

namespace Abi.Data
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct BitBuffer3
    {
        [FieldOffset(0)] private byte s1;
        [FieldOffset(1)] private byte s2;
        [FieldOffset(2)] private byte s3;

        public bool IsSet()
        {
            return s1 > 0 || s2 > 0 || s3 > 0;
        }
        public void SetAll()
        {
            s1 = BitBufferUtil.ByteMask;
            s2 = BitBufferUtil.ByteMask;
            s3 = BitBufferUtil.ByteMask;
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
