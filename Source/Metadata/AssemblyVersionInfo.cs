﻿using System.Reflection;

#if !DISABLE_ASSEMBLY_VERSION_ATTRIBUTES

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyCompany("Gapotchenko")]
[assembly: AssemblyProduct("Gapotchenko.FX")]
[assembly: AssemblyCopyright("Copyright © 2019 Gapotchenko and Contributors")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//

#if !DO_NOT_DEFINE_GLOBAL_ASSEMBLY_VERSION
[assembly: AssemblyVersion("1.1.134.16010")]
#endif
[assembly: AssemblyFileVersion("1.1.134.16010")]
[assembly: AssemblyInformationalVersion("1.1 (Beta build 1.1.134.16010)")]

#endif

[assembly: AssemblyTrademark("")]

namespace Gapotchenko.FX
{
#if ASSEMBLY_VERSION_INFO
    static partial class AssemblyVersionInfo
    {
        public const string ProductName = "Gapotchenko.FX";
        public const string CompanyName = "Gapotchenko";
        public const string InformationalVersion = "1.1 (Beta build 1.1.134.16010)";
        public const string DisplayVersion = "1.1";
        public const string DisplayVersion2F = "1.1";
    }
#endif
}