using System.Runtime.InteropServices;

namespace Abi.Data
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    internal struct BitBuffer5
    {
        [FieldOffset(0)] private uint s1;
        [FieldOffset(4)] private byte s2;

        public bool IsSet()
        {
            return s1 > 0 || s2 > 0;
        }
        public void SetAll()
        {
            s1 = BitBufferUtil.IntMask;
            s2 = BitBufferUtil.ByteMask;
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
