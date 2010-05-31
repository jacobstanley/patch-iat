using System;
using System.Runtime.InteropServices;

namespace PatchIat
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct ImageImportByName
    {
        public UInt16 Hint;
        public unsafe fixed SByte Name [1];
    }
}