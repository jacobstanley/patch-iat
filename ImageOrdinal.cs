using System;

namespace PatchIat
{
    internal static class ImageOrdinal
    {
        private const int Mask = 0xffff;
        private const Int64 Flag64 = unchecked((Int64)0x8000000000000000);
        private const Int32 Flag32 = unchecked((Int32)0x80000000);

        public static IntPtr Get(IntPtr ordinal)
        {
            if (IntPtr.Size == 4)
            {
                return new IntPtr(ordinal.ToInt32() & Mask);
            }

            return new IntPtr(ordinal.ToInt64() & Mask);
        }

        public static bool UsingOrdinals(IntPtr ordinal)
        {
            if (IntPtr.Size == 4)
            {
                return (ordinal.ToInt32() & Flag32) != 0;
            }

            return (ordinal.ToInt64() & Flag64) != 0;
        }
    }
}