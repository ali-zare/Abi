using System.Runtime.InteropServices;

namespace Abi.Data
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct BitBuffer6
    {
        [FieldOffset(0)] private ushort s1;
        [FieldOffset(2)] private ushort s2;
        [FieldOffset(4)] private ushort s3;

        public bool IsSet()
        {
            return s1 > 0 || s2 > 0 || s3 > 0;
        }
        public void SetAll()
        {
            s1 = BitBufferUtil.ShortMask;
            s2 = BitBufferUtil.ShortMask;
            s3 = BitBufferUtil.ShortMask;
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
