namespace PatchIat
{
    internal static class ImageDirectoryEntry
    {
        public const int Export = 0;
        public const int Import = 1;
        public const int Resource = 2;
        public const int Exception = 3;
        public const int Security = 4;
        public const int BaseRelocation = 5;
        public const int Debug = 6;
        public const int ArchitectureSpecific = 7;
        public const int GlobalPtr = 8;
        public const int Tls = 9;
        public const int LoadConfiguration = 10;
        public const int BoundImport = 11;
        public const int ImportAddressTable = 12;
        public const int DelayLoadImport = 13;
        public const int ComDescriptor = 14;
    }
}