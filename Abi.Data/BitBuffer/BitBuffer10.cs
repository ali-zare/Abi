using System.Runtime.InteropServices;

namespace Abi.Data
{
    [StructLayout(LayoutKind.Explicit, Pack = 2)]
    internal struct BitBuffer10
    {
        [FieldOffset(0)] private ulong s1;
        [FieldOffset(8)] private ushort s2;

        public bool IsSet()
        {
            return s1 > 0 || s2 > 0;
        }
        public void SetAll()
        {
            s1 = BitBufferUtil.LongMask;
            s2 = BitBufferUtil.ShortMask;
        }
        public void Clear()
        {
            s1 = s2 = 0;
        }

        public override string ToString()
        {
            return this.ToByteArray().ToBinaryString();
        }
    }
}
