#if DOTNET2

#region Modification Notes

// August 12 2005
// Modified by Jose Luis Barreda G. (joseluiseco 'at' hotmail 'dot' com)
// Changed name 'AssemblyCache' for 'GacHelper'
// Added 'FindAssembly' method and removed anything not needed by this method.
// Based on code from http://www.codeproject.com/csharp/GacApi.asp
// Used to simulate 'Assembly.LoadWithPartialName' (obsolete since .NET 2.0 Beta)

#endregion

#region Original notes
// Source: Microsoft KB Article KB317540

/*
SUMMARY
The native code application programming interfaces (APIs) that allow you to interact with the Global Assembly Cache (GAC) are not documented 
in the .NET Framework Software Development Kit (SDK) documentation. 

MORE INFORMATION
CAUTION: Do not use these APIs in your application to perform assembly binds or to test for the presence of assemblies or other run time, 
development, or design-time operations. Only administrative tools and setup programs must use these APIs. If you use the GAC, this directly 
exposes your application to assembly binding fragility or may cause your application to work improperly on future versions of the .NET 
Framework.

The GAC stores assemblies that are shared across all applications on a computer. The actual storage location and structure of the GAC is 
not documented and is subject to change in future versions of the .NET Framework and the Microsoft Windows operating system.

The only supported method to access assemblies in the GAC is through the APIs that are documented in this article.

Most applications do not have to use these APIs because the assembly binding is performed automatically by the common language runtime. 
Only custom setup programs or management tools must use these APIs. Microsoft Windows Installer has native support for installing assemblies
 to the GAC.

For more information about assemblies and the GAC, see the .NET Framework SDK.

Use the GAC API in the following scenarios: 
When you install nativeName assembly to the GAC.
When you remove nativeName assembly from the GAC.
When you export nativeName assembly from the GAC.
When you enumerate assemblies that are available in the GAC.
NOTE: CoInitialize(Ex) must be called before you use any of the functions and interfaces that are described in this specification. 
*/
#endregion

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Globalization;

#endregion

namespace AspectSharp.Lang
{
    /// <summary>
    /// Provides a method to find nativeName assembly in the GAC by it's simple name.
    /// </summary>
    internal static class GacHelper
    {

#region FindAssembly

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public static Assembly FindAssembly(string assemblyName)
        {
            IAssemblyEnum assemblyEnum = GacHelper.CreateGACEnum();
            IAssemblyName nativeName;

            List<AssemblyName> matches = new List<AssemblyName>();

            while(GacHelper.GetNextAssembly(assemblyEnum, out nativeName) == 0)
            {
                string nameString = GacHelper.GetName(nativeName);

                if(StringComparer.InvariantCultureIgnoreCase.Compare(nameString, assemblyName) == 0)
                {
                    AssemblyName name = GacHelper.GetAssemblyName(nativeName);
                    matches.Add(name);
                }
            }

            AssemblyName last = null;

            for(int i = 0; i < matches.Count; i++)
            {
                AssemblyName current = matches[i];

                if(last != null)
                {
                    if(last.Version.Major <= current.Version.Major)
                    {
                        if(last.Version.Major < current.Version.Major)
                        {
                            last = current;
                        }
                        else
                        {
                            if(last.Version.Minor < current.Version.Minor)
                            {
                                last = current;
                            }
                        }
                    }
                }
                else
                {
                    last = current;
                }
            }

            if(last != null)
            {
                return Assembly.Load(last);
            }
            else
            {
                return null;
            }
        }

#endregion

#region GetAssemblyName

        private static AssemblyName GetAssemblyName(IAssemblyName nameRef)
        {
            AssemblyName name = new AssemblyName();
            name.Name = GacHelper.GetName(nameRef);
            name.Version = GacHelper.GetVersion(nameRef);
            name.CultureInfo = GacHelper.GetCulture(nameRef);
            name.SetPublicKeyToken(GacHelper.GetPublicKeyToken(nameRef));

            return name;
        }

#endregion

#region GetName

        private static string GetName(IAssemblyName name)
        {
            uint bufferSize = 255;
            StringBuilder buffer = new StringBuilder((int)bufferSize);
            name.GetName(ref bufferSize, buffer);

            return buffer.ToString();
        }

#endregion

#region GetVersion

        private static Version GetVersion(IAssemblyName name)
        {
            uint majorOut;
            uint minorOut;
            name.GetVersion(out majorOut, out minorOut);

            int major = (int)majorOut >> 16;
            int minor = (int)majorOut & 0xFFFF;
            int build = (int)minorOut >> 16;
            int revision = (int)minorOut & 0xFFFF;

            if(major < 0)
            {
                major = major * -1;
            }

            if(minor < 0)
            {
                minor = minor * -1;
            }

            if(build < 0)
            {
                build = build * -1;
            }

            if(revision < 0)
            {
                revision = revision * -1;
            }

            return new Version(major, minor, build, revision);
        }

#endregion

#region GetCulture

        private static CultureInfo GetCulture(IAssemblyName name)
        {
            uint bufferSize = 255;
            IntPtr buffer = Marshal.AllocHGlobal((int)bufferSize);
            name.GetProperty(ASM_NAME.ASM_NAME_CULTURE, buffer, ref bufferSize);
            string result = Marshal.PtrToStringAuto(buffer);
            Marshal.FreeHGlobal(buffer);

            return new CultureInfo(result);
        }

#endregion

#region GetPublicKeyToken

        private static byte[] GetPublicKeyToken(IAssemblyName name)
        {
            byte[] result = new byte[8];
            uint bufferSize = 8;
            IntPtr buffer = Marshal.AllocHGlobal((int)bufferSize);
            name.GetProperty(ASM_NAME.ASM_NAME_PUBLIC_KEY_TOKEN, buffer, ref bufferSize);
            for(int i = 0; i < 8; i++)
                result[i] = Marshal.ReadByte(buffer, i);
            Marshal.FreeHGlobal(buffer);
            return result;
        }

#endregion

#region CreateGACEnum

        private static IAssemblyEnum CreateGACEnum()
        {
            IAssemblyEnum ae;

            GacHelper.CreateAssemblyEnum(out ae, (IntPtr)0, null, ASM_CACHE_FLAGS.ASM_CACHE_GAC, (IntPtr)0);

            return ae;
        }

#endregion

#region GetNextAssembly

        /// <summary>
        /// Get the next assembly name in the current enumerator or fail
        /// </summary>
        /// <param name="enumerator"></param>
        /// <param name="name"></param>
        /// <returns>0 if the enumeration is not at its end</returns>
        private static int GetNextAssembly(IAssemblyEnum enumerator, out IAssemblyName name)
        {
            return enumerator.GetNextAssembly((IntPtr)0, out name, 0);
        }

#endregion

#region External methods

        /// <summary>
        /// To obtain nativeName instance of the CreateAssemblyEnum API, call the CreateAssemblyNameObject API.
        /// </summary>
        /// <param name="pEnum">Pointer to a memory location that contains the IAssemblyEnum pointer.</param>
        /// <param name="pUnkReserved">Must be null.</param>
        /// <param name="pName">An assembly name that is used to filter the enumeration. Can be null to enumerate all assemblies in the GAC.</param>
        /// <param name="dwFlags">Exactly one bit from the ASM_CACHE_FLAGS enumeration.</param>
        /// <param name="pvReserved">Must be NULL.</param>
        [DllImport("fusion.dll", SetLastError = true, PreserveSig = false)]
        private static extern void CreateAssemblyEnum(
            out IAssemblyEnum pEnum,
            IntPtr pUnkReserved,
            IAssemblyName pName,
            ASM_CACHE_FLAGS dwFlags,
            IntPtr pvReserved);

#endregion

    }

#region Flags

#region ASM_DISPLAY_FLAGS

    /// <summary>
    /// <see cref="IAssemblyName.GetDisplayName"/>
    /// </summary>
    [Flags]
    internal enum ASM_DISPLAY_FLAGS
    {
        VERSION = 0x1,
        CULTURE = 0x2,
        PUBLIC_KEY_TOKEN = 0x4,
        PUBLIC_KEY = 0x8,
        CUSTOM = 0x10,
        PROCESSORARCHITECTURE = 0x20,
        LANGUAGEID = 0x40
    }

#endregion

#region ASM_CMP_FLAGS

    [Flags]
    internal enum ASM_CMP_FLAGS
    {
        NAME = 0x1,
        MAJOR_VERSION = 0x2,
        MINOR_VERSION = 0x4,
        BUILD_NUMBER = 0x8,
        REVISION_NUMBER = 0x10,
        PUBLIC_KEY_TOKEN = 0x20,
        CULTURE = 0x40,
        CUSTOM = 0x80,
        ALL = NAME | MAJOR_VERSION | MINOR_VERSION |
            REVISION_NUMBER | BUILD_NUMBER |
            PUBLIC_KEY_TOKEN | CULTURE | CUSTOM,
        DEFAULT = 0x100
    }

#endregion

#region ASM_NAME

    /// <summary>
    /// The ASM_NAME enumeration property ID describes the valid names of the name-value pairs in nativeName assembly name. 
    /// See the .NET Framework SDK for a description of these properties. 
    /// </summary>
    internal enum ASM_NAME
    {
        ASM_NAME_PUBLIC_KEY = 0,
        ASM_NAME_PUBLIC_KEY_TOKEN,
        ASM_NAME_HASH_VALUE,
        ASM_NAME_NAME,
        ASM_NAME_MAJOR_VERSION,
        ASM_NAME_MINOR_VERSION,
        ASM_NAME_BUILD_NUMBER,
        ASM_NAME_REVISION_NUMBER,
        ASM_NAME_CULTURE,
        ASM_NAME_PROCESSOR_ID_ARRAY,
        ASM_NAME_OSINFO_ARRAY,
        ASM_NAME_HASH_ALGID,
        ASM_NAME_ALIAS,
        ASM_NAME_CODEBASE_URL,
        ASM_NAME_CODEBASE_LASTMOD,
        ASM_NAME_NULL_PUBLIC_KEY,
        ASM_NAME_NULL_PUBLIC_KEY_TOKEN,
        ASM_NAME_CUSTOM,
        ASM_NAME_NULL_CUSTOM,
        ASM_NAME_MVID,
        ASM_NAME_MAX_PARAMS
    }

#endregion

#region ASM_CACHE_FLAGS

    /// <summary>
    /// The ASM_CACHE_FLAGS enumeration contains the following values: 
    /// ASM_CACHE_ZAP - Enumerates the cache of precompiled assemblies by using Ngen.exe.
    /// ASM_CACHE_GAC - Enumerates the GAC.
    /// ASM_CACHE_DOWNLOAD - Enumerates the assemblies that have been downloaded on-demand or that have been shadow-copied.
    /// </summary>
    [Flags]
    internal enum ASM_CACHE_FLAGS
    {
        ASM_CACHE_ZAP = 0x1,
        ASM_CACHE_GAC = 0x2,
        ASM_CACHE_DOWNLOAD = 0x4
    }

#endregion

#endregion

#region IAssemblyName interface

    /// <summary>
    /// The IAssemblyName interface represents nativeName assembly name. An assembly name includes a predetermined set of name-value pairs. 
    /// The assembly name is described in detail in the .NET Framework SDK.
    /// </summary>
    [ComImport, Guid("CD193BC0-B4BC-11d2-9833-00C04FC31D2E"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAssemblyName
    {
        /// <summary>
        /// The IAssemblyName::SetProperty method adds a name-value pair to the assembly name, or, if a name-value pair 
        /// with the same name already exists, modifies or deletes the value of a name-value pair.
        /// </summary>
        /// <param name="PropertyId">The ID that represents the name part of the name-value pair that is to be 
        /// added or to be modified. Valid property IDs are defined in the ASM_NAME enumeration.</param>
        /// <param name="pvProperty">A pointer to a buffer that contains the value of the property.</param>
        /// <param name="cbProperty">The length of the pvProperty buffer in bytes. If cbProperty is zero, the name-value pair 
        /// is removed from the assembly name.</param>
        /// <returns></returns>
        [PreserveSig]
        int SetProperty(
            ASM_NAME PropertyId,
            IntPtr pvProperty,
            uint cbProperty);

        /// <summary>
        /// The IAssemblyName::GetProperty method retrieves the value of a name-value pair in the assembly name that specifies the name.
        /// </summary>
        /// <param name="PropertyId">The ID that represents the name of the name-value pair whose value is to be retrieved.
        /// Specified property IDs are defined in the ASM_NAME enumeration.</param>
        /// <param name="pvProperty">A pointer to a buffer that is to contain the value of the property.</param>
        /// <param name="pcbProperty">The length of the pvProperty buffer, in bytes.</param>
        /// <returns></returns>
        [PreserveSig]
        int GetProperty(
            ASM_NAME PropertyId,
            IntPtr pvProperty,
            ref uint pcbProperty);

        /// <summary>
        /// The IAssemblyName::Finalize method freezes nativeName assembly name. Additional calls to IAssemblyName::SetProperty are 
        /// unsuccessful after this method has been called.
        /// </summary>
        /// <returns></returns>
        [PreserveSig]
        int Finalize();

        /// <summary>
        /// The IAssemblyName::GetDisplayName method returns a string representation of the assembly name.
        /// </summary>
        /// <param name="szDisplayName">A pointer to a buffer that is to contain the display name. The display name is returned in Unicode.</param>
        /// <param name="pccDisplayName">The size of the buffer in characters (on input). The length of the returned display name (on return).</param>
        /// <param name="dwDisplayFlags">One or more of the bits defined in the ASM_DISPLAY_FLAGS enumeration: 
        ///		*_VERSION - Includes the version number as part of the display name.
        ///		*_CULTURE - Includes the culture.
        ///		*_PUBLIC_KEY_TOKEN - Includes the public key token.
        ///		*_PUBLIC_KEY - Includes the public key.
        ///		*_CUSTOM - Includes the custom part of the assembly name.
        ///		*_PROCESSORARCHITECTURE - Includes the processor architecture.
        ///		*_LANGUAGEID - Includes the language ID.</param>
        /// <returns></returns>
        /// <remarks>http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpguide/html/cpcondefaultmarshalingforstrings.asp</remarks>
        [PreserveSig]
        int GetDisplayName(
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder szDisplayName,
            ref uint pccDisplayName,
            ASM_DISPLAY_FLAGS dwDisplayFlags);

        /// <summary>
        /// Undocumented
        /// </summary>
        /// <param name="refIID"></param>
        /// <param name="pUnkSink"></param>
        /// <param name="pUnkContext"></param>
        /// <param name="szCodeBase"></param>
        /// <param name="llFlags"></param>
        /// <param name="pvReserved"></param>
        /// <param name="cbReserved"></param>
        /// <param name="ppv"></param>
        /// <returns></returns>
        [PreserveSig]
        int BindToObject(
            ref Guid refIID,
            [MarshalAs(UnmanagedType.IUnknown)] object pUnkSink,
            [MarshalAs(UnmanagedType.IUnknown)] object pUnkContext,
            [MarshalAs(UnmanagedType.LPWStr)] string szCodeBase,
            long llFlags,
            IntPtr pvReserved,
            uint cbReserved,
            out IntPtr ppv);

        /// <summary>
        /// The IAssemblyName::GetName method returns the name part of the assembly name.
        /// </summary>
        /// <param name="lpcwBuffer">Size of the pwszName buffer (on input). Length of the name (on return).</param>
        /// <param name="pwzName">Pointer to the buffer that is to contain the name part of the assembly name.</param>
        /// <returns></returns>
        /// <remarks>http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpguide/html/cpcondefaultmarshalingforstrings.asp</remarks>
        [PreserveSig]
        int GetName(
            ref uint lpcwBuffer,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwzName);

        /// <summary>
        /// The IAssemblyName::GetVersion method returns the version part of the assembly name.
        /// </summary>
        /// <param name="pdwVersionHi">Pointer to a DWORD that contains the upper 32 bits of the version number.</param>
        /// <param name="pdwVersionLow">Pointer to a DWORD that contain the lower 32 bits of the version number.</param>
        /// <returns></returns>
        [PreserveSig]
        int GetVersion(
            out uint pdwVersionHi,
            out uint pdwVersionLow);

        /// <summary>
        /// The IAssemblyName::IsEqual method compares the assembly name to another assembly names.
        /// </summary>
        /// <param name="pName">The assembly name to compare to.</param>
        /// <param name="dwCmpFlags">Indicates which part of the assembly name to use in the comparison. 
        /// Values are one or more of the bits defined in the ASM_CMP_FLAGS enumeration.</param>
        /// <returns></returns>
        [PreserveSig]
        int IsEqual(
            IAssemblyName pName,
            ASM_CMP_FLAGS dwCmpFlags);

        /// <summary>
        /// The IAssemblyName::Clone method creates a copy of nativeName assembly name. 
        /// </summary>
        /// <param name="pName"></param>
        /// <returns></returns>
        [PreserveSig]
        int Clone(
            out IAssemblyName pName);
    }

#endregion

#region IAssemblyEnum interface

    /// <summary>
    /// The IAssemblyEnum interface enumerates the assemblies in the GAC.
    /// </summary>
    [ComImport, Guid("21b8916c-f28e-11d2-a473-00c04f8ef448"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAssemblyEnum
    {
        /// <summary>
        /// The IAssemblyEnum::GetNextAssembly method enumerates the assemblies in the GAC. 
        /// </summary>
        /// <param name="pvReserved">Must be null.</param>
        /// <param name="ppName">Pointer to a memory location that is to receive the interface pointer to the assembly 
        /// name of the next assembly that is enumerated.</param>
        /// <param name="dwFlags">Must be zero.</param>
        /// <returns></returns>
        [PreserveSig()]
        int GetNextAssembly(
            IntPtr pvReserved,
            out IAssemblyName ppName,
            uint dwFlags);

        /// <summary>
        /// Undocumented. Best guess: reset the enumeration to the first assembly.
        /// </summary>
        /// <returns></returns>
        [PreserveSig()]
        int Reset();

        /// <summary>
        /// Undocumented. Create a copy of the assembly enum that is independently enumerable.
        /// </summary>
        /// <param name="ppEnum"></param>
        /// <returns></returns>
        [PreserveSig()]
        int Clone(
            out IAssemblyEnum ppEnum);
    }

#endregion

}
#endif