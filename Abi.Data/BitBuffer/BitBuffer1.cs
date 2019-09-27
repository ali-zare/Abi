﻿using System.Runtime.InteropServices;

namespace Abi.Data
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct BitBuffer1
    {
        [FieldOffset(0)] private byte s1;

        public bool IsSet()
        {
            return s1 > 0;
        }
        public void SetAll()
        {
            s1 = BitBufferUtil.ByteMask;
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
