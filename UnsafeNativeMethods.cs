using System;
using System.Runtime.InteropServices;

namespace PatchIat
{
    internal static class UnsafeNativeMethods
    {
        /// <summary>
        /// Retrieves information about a range of pages in the virtual address
        /// space of the calling process.
        /// </summary>
        /// <param name="lpAddress">
        /// A pointer to the base address of the region of pages to be queried.
        /// This value is rounded down to the next page boundary.
        /// </param>
        /// <param name="lpBuffer">
        /// A pointer to a <see cref="MemoryBasicInformation"/> structure in
        /// which information about the specified page range is returned.
        /// </param>
        /// <param name="dwLength">
        /// The size of the buffer pointed to by the <paramref name="lpBuffer"/>
        /// parameter, in bytes.
        /// </param>
        /// <returns>
        /// The actual number of bytes returned in the information buffer.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern unsafe IntPtr VirtualQuery(
            [In, Optional] IntPtr lpAddress,
            [Out] MemoryBasicInformation* lpBuffer,
            [In] IntPtr dwLength);

        /// <summary>
        /// Changes the protection on a region of committed pages in the virtual
        /// address space of the calling process.
        /// </summary>
        /// <param name="lpAddress">
        /// A pointer to the base address of the region of pages whose access
        /// protection attributes are to be changed.
        /// </param>
        /// <param name="dwSize">
        /// The size of the region whose access protection attributes are to be
        /// changed, in bytes. The region of affected pages includes all pages
        /// containing one or more bytes in the range from the
        /// <paramref name="lpAddress"/> parameter to (<paramref name="lpAddress"/>
        /// + <paramref name="dwSize"/>). This means that a 2-byte range straddling
        /// a page boundary causes the protection attributes of both pages to be changed.
        /// </param>
        /// <param name="flNewProtect">
        /// The memory protection option.
        /// </param>
        /// <param name="lpflOldProtect">
        /// A pointer to a variable that receives the previous access protection value of
        /// the first page in the specified region of pages. If this parameter is <c>null</c>
        /// or does not point to a valid variable, the function fails.
        /// </param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern unsafe bool VirtualProtect(
            [In] IntPtr lpAddress,
            [In] IntPtr dwSize,
            [In] MemoryProtectionOptions flNewProtect,
            [Out] MemoryProtectionOptions* lpflOldProtect);
    }
}