namespace PatchIat
{
    internal enum ImageHeaderType : ushort
    {
        Invalid = 0x0,
        Executable32Bit = 0x10b,
        Executable64Bit = 0x20b,
        Rom = 0x107,
    }
}