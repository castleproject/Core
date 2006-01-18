using System.Reflection;

[assembly : AssemblyTitle("NVelocity")]
[assembly : AssemblyDescription("")]
[assembly : AssemblyCompany("")]
[assembly : AssemblyProduct("")]
[assembly : AssemblyCopyright("")]
[assembly : AssemblyTrademark("")]
[assembly : AssemblyCulture("")]

#if STRONG
[assembly: AssemblyKeyFile("../../CastleKey.snk")]
#elif VISUALSTUDIO7
[assembly: AssemblyKeyFile(@"../../../../../../CastleKey.snk")]
//[assembly: AssemblyKeyFile(@"../../svn/CastleKey.snk")]
#endif
