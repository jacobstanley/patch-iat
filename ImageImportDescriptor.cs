using System;
using System.Runtime.InteropServices;

namespace PatchIat
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct ImageImportDescriptor
    {
        [FieldOffset(0)]
        public UInt32 Characteristics;

        [FieldOffset(0)]
        public UInt32 OriginalFirstThunk;

        [FieldOffset(1 * sizeof (UInt32))]
        public UInt32 TimeDateStamp;

        [FieldOffset(2 * sizeof (UInt32))]
        public UInt32 ForwarderChain;

        [FieldOffset(3 * sizeof (UInt32))]
        public UInt32 Name;

        [FieldOffset(4 * sizeof (UInt32))]
        public UInt32 FirstThunk;
    }
}