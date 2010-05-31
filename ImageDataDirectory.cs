using System;
using System.Runtime.InteropServices;

namespace PatchIat
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct ImageDataDirectory
    {
        public UInt32 VirtualAddress;
        public UInt32 Size;
    }
}