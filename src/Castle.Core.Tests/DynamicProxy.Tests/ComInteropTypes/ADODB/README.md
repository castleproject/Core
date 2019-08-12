# Note about the types defined in `Castle.DynamicProxy.Tests.ComInteropTypes.ADODB`

The `Castle.DynamicProxy.Tests.ComInteropTypes.ADODB` namespace contains COM interop type declarations that have been extracted from a COM interop assembly, `ADODB.dll`, which is generally available on Windows systems.

These interop type definitions are incomplete, and that they may contain inaccuracies. They are only reproduced to the approximate extent necessary to run the tests depending on them. Hints (in the form of code comments) are given where omissions or truncations were made.

We used to reference types in the original `ADODB.dll` directly. The reason why we no longer do this, but instead reproduce the types here, is that the DLL is bound against the .NET Framework 3.5 DLL, while our test projects target .NET Framework 4.x. This can lead to assembly load failure e.g. on our Mono/Linux CI build. By moving the COM interop type definitions into this project, we can get rid of stale .NET 3.5 assembly references.
