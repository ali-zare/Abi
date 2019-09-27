namespace Abi.Data
{
    public enum ForeignStatus : byte
    {
        Unkhown = 0x0,                  // 00000000

        Invalid = 0x1,                  // 00000001
        KeyNotFound = 0x3,              // 00000011
        RefKeyIsZero = 0x5,             // 00000101
        RefKeyNotEqualKey = 0x9,        // 00001001
        RefNotFound = 0x11,             // 00010001
        RefStateNotEditable = 0x21,     // 00100001
        RefToNotExistEntity = 0x41,     // 01000001
        RefToNonUniqueEntity = 0x81,    // 10000001

        Null = 0x2,                     // 00000010
        Orphan = 0x4,                   // 00000100
        UpdateRef = 0x8,                // 00001000
        UpdateKey = 0x10,               // 00010000
        Complete = 0x20,                // 00100000
    }
}