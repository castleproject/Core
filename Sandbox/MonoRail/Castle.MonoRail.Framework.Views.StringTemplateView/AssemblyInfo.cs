using System.Reflection;
using System.Runtime.CompilerServices;

//
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//
[assembly: AssemblyTitle("ST View Engine for Castle.MonoRail")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]		


[assembly: AssemblyVersion("1.0.0.0")]

#if STRONG
[assembly: AssemblyKeyFile("../../CastleKey.snk")]
[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyName("")]
#else
// This assembly must be signed, so if STRONG is not defined
// we assume this compilation is being held by Visual Studio
// [assembly: AssemblyKeyFile("../../../../CastleKey.snk")]
[assembly: AssemblyKeyFile("../../../../../CastleKey.snk")]
#endif
