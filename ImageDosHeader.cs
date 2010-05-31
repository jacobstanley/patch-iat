using System;
using System.Runtime.InteropServices;

namespace PatchIat
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct ImageDosHeader
    {
        ///<summary>Magic number</summary>
        public ImageDosSignature e_magic;

        ///<summary>Bytes on last page of file</summary>
        public UInt16 e_cblp;

        ///<summary>Pages in file</summary>
        public UInt16 e_cp;

        ///<summary>Relocations</summary>
        public UInt16 e_crlc;

        ///<summary>Size of header in paragraphs</summary>
        public UInt16 e_cparhdr;

        ///<summary>Minimum extra paragraphs needed</summary>
        public UInt16 e_minalloc;

        ///<summary>Maximum extra paragraphs needed</summary>
        public UInt16 e_maxalloc;

        ///<summary>Initial (relative) SS value</summary>
        public UInt16 e_ss;

        ///<summary>Initial SP value</summary>
        public UInt16 e_sp;

        ///<summary>Checksum</summary>
        public UInt16 e_csum;

        ///<summary>Initial IP value</summary>
        public UInt16 e_ip;

        ///<summary>Initial (relative) CS value</summary>
        public UInt16 e_cs;

        ///<summary>File address of relocation table</summary>
        public UInt16 e_lfarlc;

        ///<summary>Overlay number</summary>
        public UInt16 e_ovno;

        ///<summary>Reserved words</summary>
        public unsafe fixed UInt16 e_res1 [4];

        ///<summary>OEM identifier (for e_oeminfo)</summary>
        public UInt16 e_oemid;

        ///<summary>OEM information; e_oemid specific</summary>
        public UInt16 e_oeminfo;

        /// <summary>Reserved words</summary>
        public unsafe fixed UInt16 e_res2 [10];

        /// <summary>File address of new exe header</summary>
        public Int32 e_lfanew;

        public void VerifySignature()
        {
            if (e_magic != ImageDosSignature.MZ)
            {
                throw new BadImageFormatException();
            }
        }
    }
}