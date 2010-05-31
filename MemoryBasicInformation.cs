using System;
using System.Runtime.InteropServices;

namespace PatchIat
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct MemoryBasicInformation
    {
        public IntPtr BaseAddress;
        public IntPtr AllocationBase;
        public UInt32 AllocationProtect;
        public IntPtr RegionSize;
        public UInt32 State;
        public MemoryProtectionOptions Protect;
        public UInt32 Type;
    }
}