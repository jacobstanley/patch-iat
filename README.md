# What is this?

PatchIat provides the ability to patch the Import Address Table and hook
and function imported by a dll or executable, provided you can get access
to its ProcessModule, the .NET wrapper for HMODULE.

# Example

You can find and patch an import using the following code:

    ProcessModule module = Process
        .GetCurrentProcess().Modules.Cast<ProcessModule>()
        .Where(x => "some.dll".Equals(Path.GetFileName(x.FileName), StringComparison.OrdinalIgnoreCase))
        .FirstOrDefault();

    if (module != null)
    {
        module.Patch(
            "Gdi32.dll",
            "CreateFontIndirectA",
            (CreateFontIndirectA original) => font =>
            {
                font->lfQuality = NONANTIALIASED_QUALITY;
                return original(font);
            });
    }

    // supporting types

    private unsafe delegate IntPtr CreateFontIndirectA(LOGFONTA* lplf);

    private const int NONANTIALIASED_QUALITY = 3;

    [StructLayout(LayoutKind.Sequential)]
    private struct LOGFONTA
    {
        public int lfHeight;
        public int lfWidth;
        public int lfEscapement;
        public int lfOrientation;
        public int lfWeight;
        public byte lfItalic;
        public byte lfUnderline;
        public byte lfStrikeOut;
        public byte lfCharSet;
        public byte lfOutPrecision;
        public byte lfClipPrecision;
        public byte lfQuality;
        public byte lfPitchAndFamily;
        public unsafe fixed sbyte lfFaceName [32];
    }

# License

PatchIat is licensed under the MIT-Zero License, a modified version of the
MIT license that does not require the copyright notice to be included when
copying the software.

Basically, do whatever you want with this code. If you want to copy and paste
this stuff straight in to your project so you don't need to reference another
assembly, then that's fine by me :)

