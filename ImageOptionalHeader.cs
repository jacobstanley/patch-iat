using System;
using System.Runtime.InteropServices;

namespace PatchIat
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct ImageOptionalHeader
    {
        //
        // Standard fields.
        //
        public ImageHeaderType Magic;
        public Byte MajorLinkerVersion;
        public Byte MinorLinkerVersion;
        public UInt32 SizeOfCode;
        public UInt32 SizeOfInitializedData;
        public UInt32 SizeOfUninitializedData;
        public UInt32 AddressOfEntryPoint;
        public UInt32 BaseOfCode;
        public UInt32 BaseOfData;

        //
        // NT additional fields.
        //
        public IntPtr ImageBase;
        public UInt32 SectionAlignment;
        public UInt32 FileAlignment;
        public UInt16 MajorOperatingSystemVersion;
        public UInt16 MinorOperatingSystemVersion;
        public UInt16 MajorImageVersion;
        public UInt16 MinorImageVersion;
        public UInt16 MajorSubsystemVersion;
        public UInt16 MinorSubsystemVersion;
        public UInt32 Win32VersionValue;
        public UInt32 SizeOfImage;
        public UInt32 SizeOfHeaders;
        public UInt32 CheckSum;
        public UInt16 Subsystem;
        public UInt16 DllCharacteristics;
        public IntPtr SizeOfStackReserve;
        public IntPtr SizeOfStackCommit;
        public IntPtr SizeOfHeapReserve;
        public IntPtr SizeOfHeapCommit;
        public UInt32 LoaderFlags;
        public UInt32 NumberOfRvaAndSizes;
        public unsafe fixed UInt64 DataDirectory [16];

        public unsafe ImageDataDirectory GetDataDirectory(int index)
        {
            fixed (UInt64* dataDirectoryPtr = DataDirectory)
            {
                return ((ImageDataDirectory*)dataDirectoryPtr)[index];
            }
        }
    }
}