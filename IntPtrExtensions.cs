using System;
using System.Diagnostics;

namespace PatchIat
{
    internal static class IntPtrExtensions
    {
        public static IntPtr AsPtr(this Int32 relativeAddress, ProcessModule module)
        {
            return new IntPtr(module.BaseAddress.ToInt64() + relativeAddress);
        }

        public static IntPtr AsPtr(this UInt32 relativeAddress, ProcessModule module)
        {
            return new IntPtr(module.BaseAddress.ToInt64() + relativeAddress);
        }

        public static IntPtr AsPtr(this IntPtr relativeAddress, ProcessModule module)
        {
            return new IntPtr(module.BaseAddress.ToInt64() + relativeAddress.ToInt64());
        }
    }
}