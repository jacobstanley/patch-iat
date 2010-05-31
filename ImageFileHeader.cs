using System;
using System.Runtime.InteropServices;

namespace PatchIat
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct ImageFileHeader
    {
        public ImageArchitecture Machine;
        public UInt16 NumberOfSections;
        public UInt32 TimeDateStamp;
        public UInt32 PointerToSymbolTable;
        public UInt32 NumberOfSymbols;
        public UInt16 SizeOfOptionalHeader;
        public UInt16 Characteristics;
    }
}