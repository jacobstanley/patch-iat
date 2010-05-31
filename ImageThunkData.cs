using System;
using System.Runtime.InteropServices;

namespace PatchIat
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct ImageThunkData
    {
        [FieldOffset(0)]
        public IntPtr ForwarderString;

        [FieldOffset(0)]
        public IntPtr Function;

        [FieldOffset(0)]
        public IntPtr Ordinal;

        [FieldOffset(0)]
        public IntPtr AddressOfData;
    }
}