using System.Runtime.InteropServices;

namespace Abi.Data
{
    [StructLayout(LayoutKind.Explicit)]
    internal partial struct BitBuffer2
    {
        [FieldOffset(0)] private ushort s1;

        public bool IsSet()
        {
            return s1 > 0;
        }
        public void SetAll()
        {
            s1 = BitBufferUtil.ShortMask;
        }
        public void Clear()
        {
            s1 = 0;
        }

        public override string ToString()
        {
            return this.ToByteArray().ToBinaryString();
        }
    }
}
