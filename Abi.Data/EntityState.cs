using System;

namespace Abi.Data
{
    [Flags]
    public enum EntityState : byte
    {
        Detached = 0x0,     // 00 00 0000

        Editable = 0x1,     // 00 00 0001
        Trackable = 0x2,    // 00 00 0010
        Changed = 0x4,      // 00 00 0100
        Busy = 0x8,         // 00 00 1000

        Unchanged = 0x13,   // 00 01 0011
        Modified = 0x27,    // 00 10 0111
        Added = 0x15,       // 00 01 0101
        LateAdd = 0x10,     // 00 01 0000
        Deleted = 0x20,     // 00 10 0000

        Editing = 0x48,     // 01 00 1000
        Referencing = 0x88, // 10 00 1000
    }
}
