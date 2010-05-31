using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PatchIat
{
    /// <summary>
    /// Provides methods for patching a module's import address table.
    /// </summary>
    public static unsafe class ImportAddressTable
    {
        /// <summary>
        /// Patches the specified module's import address table.
        /// </summary>
        /// <typeparam name="TFunction">The delegate type of the function to patch.</typeparam>
        /// <param name="module">The module to patch.</param>
        /// <param name="importedModule">The imported module (eg. gdi32.dll)</param>
        /// <param name="importedFunction">The imported function (eg. CreateFontIndirectA)</param>
        /// <param name="buildReplacementFunction">
        /// A builder function which takes the original function as an argument and
        /// returns the function to replace it with.
        /// </param>
        public static void Patch<TFunction>(
            this ProcessModule module,
            string importedModule,
            string importedFunction,
            Func<TFunction, TFunction> buildReplacementFunction)
            where TFunction : class
        {
            PreventGarbageCollection(
                ApplyPatch(
                    module,
                    importedModule,
                    importedFunction,
                    CreatePredicate(importedFunction),
                    buildReplacementFunction));
        }

        /// <summary>
        /// Patches the specified module's import address table.
        /// </summary>
        /// <typeparam name="TFunction">The delegate type of the function to patch.</typeparam>
        /// <param name="module">The module to patch.</param>
        /// <param name="importedModule">The imported module (eg. gdi32.dll)</param>
        /// <param name="importedOrdinal">The ordinal number of the imported function.</param>
        /// <param name="buildReplacementFunction">A builder function which takes the original function as an argument and
        /// returns the function to replace it with.</param>
        public static void Patch<TFunction>(
            this ProcessModule module,
            string importedModule,
            int importedOrdinal,
            Func<TFunction, TFunction> buildReplacementFunction)
            where TFunction : class
        {
            PreventGarbageCollection(
                ApplyPatch(
                    module,
                    importedModule,
                    "Ordinal " + importedOrdinal,
                    CreatePredicate(importedOrdinal),
                    buildReplacementFunction));
        }

        private static readonly List<PatchedFunction> s_PatchedFunctions = new List<PatchedFunction>();

        private static void PreventGarbageCollection(PatchedFunction patchedFunction)
        {
            lock (s_PatchedFunctions)
            {
                s_PatchedFunctions.Add(patchedFunction);
            }
        }

        private static Patched<TFunction> ApplyPatch<TFunction>(
            ProcessModule module,
            string importedModule,
            string importedName,
            ThunkPredicate isCorrectFunction,
            Func<TFunction, TFunction> buildReplacementFunction)
            where TFunction : class
        {
            AssertDelegate<TFunction>("TFunction");

            module.NotNull("module");
            importedModule.NotEmpty("importedModule");
            isCorrectFunction.NotNull("isCorrectFunction");
            buildReplacementFunction.NotNull("buildReplacementFunction");

            ImageImportDescriptor* descriptors = module.GetImportDescriptors();
            ImageImportDescriptor* descriptor = module.FindImportedModule(descriptors, importedModule);
            ImageThunkData* thunk = module.FindImportedFunction(descriptor, isCorrectFunction);
            MemoryBasicInformation thunkMemory = GetMemoryInformation(thunk);

            MakeWritable(thunkMemory);

            try
            {
                TFunction originalFunction = GetDelegateForFunctionPointer<TFunction>(thunk->Function);

                if (originalFunction == null)
                {
                    throw new ArgumentException("Could not create a delegate for the original function to be patched.");
                }

                TFunction replacementFunction = buildReplacementFunction(originalFunction);
                thunk->Function = GetFunctionPointerForDelegate(replacementFunction);

                return PatchedFunction.From(
                    importedModule,
                    importedName,
                    replacementFunction,
                    originalFunction);
            }
            finally
            {
                RestoreOriginalProtection(thunkMemory);
            }
        }

        private static ImageThunkData* FindImportedFunction(
            this ProcessModule module,
            ImageImportDescriptor* descriptor,
            ThunkPredicate isCorrectFunction)
        {
            if (descriptor->FirstThunk == 0 ||
                descriptor->OriginalFirstThunk == 0)
            {
                throw new ArgumentException("The thunks in the specified descriptor were null.", "descriptor");
            }

            ImageThunkData* first = (ImageThunkData*)descriptor->FirstThunk.AsPtr(module);
            ImageThunkData* original = (ImageThunkData*)descriptor->OriginalFirstThunk.AsPtr(module);

            for (; original->Function != IntPtr.Zero; original++, first++)
            {
                if (isCorrectFunction(module, original))
                {
                    return first;
                }
            }

            throw new MissingMethodException();
        }

        private delegate bool ThunkPredicate(ProcessModule module, ImageThunkData* thunk);

        private static ThunkPredicate CreatePredicate(string function)
        {
            return (module, thunk) =>
            {
                if (ImageOrdinal.UsingOrdinals(thunk->Ordinal))
                {
                    return false;
                }

                ImageImportByName* import = (ImageImportByName*)thunk->AddressOfData.AsPtr(module);
                string name = new string(import->Name);

                if (String.Equals(function, name, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                return false;
            };
        }

        private static ThunkPredicate CreatePredicate(int ordinal)
        {
            return (module, thunk) =>
            {
                if (!ImageOrdinal.UsingOrdinals(thunk->Ordinal))
                {
                    return false;
                }

                return ImageOrdinal.Get(thunk->Ordinal) == new IntPtr(ordinal);
            };
        }

        private static ImageImportDescriptor* FindImportedModule(
            this ProcessModule module,
            ImageImportDescriptor* descriptors,
            string importedModuleName)
        {
            while (descriptors->Characteristics != 0)
            {
                sbyte* currentNamePtr = (sbyte*)descriptors->Name.AsPtr(module);
                string currentName = new string(currentNamePtr);

                if (String.Equals(currentName, importedModuleName, StringComparison.OrdinalIgnoreCase))
                {
                    return descriptors;
                }

                descriptors++;
            }

            throw new DllNotFoundException();
        }

        private static void MakeWritable(MemoryBasicInformation thunkMemoryInfo)
        {
            if (!UnsafeNativeMethods.VirtualProtect(
                thunkMemoryInfo.BaseAddress,
                thunkMemoryInfo.RegionSize,
                MemoryProtectionOptions.ExecuteReadWrite,
                &thunkMemoryInfo.Protect))
            {
                throw new Win32Exception();
            }
        }

        private static void RestoreOriginalProtection(MemoryBasicInformation thunkMemoryInfo)
        {
            MemoryProtectionOptions ignore;

            if (!UnsafeNativeMethods.VirtualProtect(
                thunkMemoryInfo.BaseAddress,
                thunkMemoryInfo.RegionSize,
                thunkMemoryInfo.Protect,
                &ignore))
            {
                throw new Win32Exception();
            }
        }

        private static MemoryBasicInformation GetMemoryInformation(ImageThunkData* thunk)
        {
            MemoryBasicInformation thunkMemoryInfo;

            UnsafeNativeMethods.VirtualQuery(
                (IntPtr)thunk,
                &thunkMemoryInfo,
                (IntPtr)sizeof (MemoryBasicInformation));

            return thunkMemoryInfo;
        }

        private static ImageImportDescriptor* GetImportDescriptors(this ProcessModule module)
        {
            ImageNTHeaders* ntHeader = module.GetNTHeader();

            ImageDataDirectory importDataDirectory =
                ntHeader->OptionalHeader.GetDataDirectory(ImageDirectoryEntry.Import);

            return (ImageImportDescriptor*)importDataDirectory.VirtualAddress.AsPtr(module);
        }

        private static ImageNTHeaders* GetNTHeader(this ProcessModule module)
        {
            ImageDosHeader* dos = (ImageDosHeader*)module.BaseAddress;
            dos->VerifySignature();

            ImageNTHeaders* nt = (ImageNTHeaders*)dos->e_lfanew.AsPtr(module);
            nt->VerifySignature();

            return nt;
        }

        private static T GetDelegateForFunctionPointer<T>(IntPtr ptr)
        {
            return (T)(object)Marshal.GetDelegateForFunctionPointer(ptr, typeof (T));
        }

        private static IntPtr GetFunctionPointerForDelegate<T>(T @delegate)
        {
            return Marshal.GetFunctionPointerForDelegate((Delegate)(object)@delegate);
        }

        private static void AssertDelegate<T>(string genericArgumentName)
        {
            if (!typeof (Delegate).IsAssignableFrom(typeof (T)))
            {
                throw new InvalidCastException("The type of " + genericArgumentName + " must be a Delegate (was " + typeof (T).Name + ")");
            }
        }

        #region PatchedFunction

        private abstract class PatchedFunction
        {
            private readonly string m_Module;
            private readonly string m_Name;

            protected PatchedFunction(string moduleName, string name)
            {
                m_Module = moduleName.NotEmpty("moduleName");
                m_Name = name.NotEmpty("name");
            }

            public override string ToString()
            {
                return m_Name + " (" + m_Module + ")";
            }

            public static Patched<TFunction> From<TFunction>(string module, string function, TFunction replacement, TFunction original) where TFunction : class
            {
                return new Patched<TFunction>(module, function, replacement, original);
            }
        }

        private sealed class Patched<TFunction> : PatchedFunction
            where TFunction : class
        {
            //
            // ReSharper disable UnaccessedField.Local
            //
            // These are fields required to prevent the replacement
            // function from being garbage collected. The original
            // function is kept for completeness.
            //

            private readonly TFunction m_Replacement;
            private readonly TFunction m_Original;

            //
            // ReSharper restore UnaccessedField.Local
            //

            public Patched(string module, string name, TFunction patched, TFunction original)
                : base(module, name)
            {
                AssertDelegate<TFunction>("TFunction");
                m_Replacement = patched.NotNull("patched");
                m_Original = original.NotNull("original");
            }
        }

        #endregion
    }
}