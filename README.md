# Castle Core

<img align="right" src="docs/images/castle-logo.png">

Castle Core provides common Castle Project abstractions including logging services. It also features **Castle DynamicProxy** a lightweight runtime proxy generator, and **Castle DictionaryAdapter**.

See the [documentation](docs/README.md).

## Releases

See the [Releases](https://github.com/castleproject/Core/releases).

## License

Castle Core is &copy; 2004-2016 Castle Project. It is free software, and may be redistributed under the terms of the [Apache 2.0](http://opensource.org/licenses/Apache-2.0) license.

## Building

### .NET Framework and .NET Core

```
msbuild /p:Configuration=NET35-Release /t:RunAllTests buildscripts/Build.proj
msbuild /p:Configuration=NET40-Release /t:RunAllTests buildscripts/Build.proj
msbuild /p:Configuration=NET45-Release /t:RunAllTests buildscripts/Build.proj
build.cmd NETCORE
```

### Mono

Castle Core works with some limitations and defects on Mono 4.0.2+, however Mono 4.6.1+ is highly
recommended, Mono 4.0.0 and 4.0.1 have serious runtime bugs that cause runtime crashes. Mono 3.x
releases used to work well, but are not supported.
Check [our issue tracker](https://github.com/castleproject/Core/issues?utf8=%E2%9C%93&q=is%3Aissue%20is%3Aopen%20mono) for known Mono defects.

```
xbuild /p:Configuration=NET45-Release /t:RunAllTests buildscripts/Build.proj
```

### Conditional Compilation Symbols

The following conditional compilation symbols (vertical) are currently defined for each of the build configurations (horizontal):

Symbol                              | NET35              | NET40              | NET45              | .NET Core
----------------------------------- | ------------------ | ------------------ | ------------------ | ------------------
`FEATURE_APPDOMAIN`                 | :white_check_mark: | :white_check_mark: | :white_check_mark: | :no_entry_sign:
`FEATURE_APTCA`                     | :no_entry_sign:    | :no_entry_sign:    | :no_entry_sign:    | :no_entry_sign:
`FEATURE_ASSEMBLYBUILDER_SAVE`      | :white_check_mark: | :white_check_mark: | :white_check_mark: | :no_entry_sign:
`FEATURE_BINDINGLIST`               | :white_check_mark: | :white_check_mark: | :white_check_mark: | :no_entry_sign:
`FEATURE_DICTIONARYADAPTER_XML`     | :white_check_mark: | :white_check_mark: | :white_check_mark: | :no_entry_sign:
`FEATURE_EMIT_CUSTOMMODIFIERS`      | :white_check_mark: | :white_check_mark: | :white_check_mark: | :no_entry_sign:
`FEATURE_EVENTLOG`                  | :white_check_mark: | :white_check_mark: | :white_check_mark: | :no_entry_sign:
`FEATURE_GAC`                       | :white_check_mark: | :white_check_mark: | :white_check_mark: | :no_entry_sign:
`FEATURE_GET_REFERENCED_ASSEMBLIES` | :white_check_mark: | :white_check_mark: | :white_check_mark: | :no_entry_sign:
`FEATURE_IDATAERRORINFO`            | :white_check_mark: | :white_check_mark: | :white_check_mark: | :no_entry_sign:
`FEATURE_ISUPPORTINITIALIZE`        | :white_check_mark: | :white_check_mark: | :white_check_mark: | :no_entry_sign:
`FEATURE_LEGACY_REFLECTION_API`     | :white_check_mark: | :white_check_mark: | :no_entry_sign:    | :no_entry_sign:
`FEATURE_LISTSORT`                  | :white_check_mark: | :white_check_mark: | :white_check_mark: | :no_entry_sign:
`FEATURE_NETCORE_REFLECTION_API`    | :no_entry_sign:    | :no_entry_sign:    | :no_entry_sign:    | :white_check_mark:
`FEATURE_REMOTING`                  | :white_check_mark: | :white_check_mark: | :white_check_mark: | :no_entry_sign:
`FEATURE_SECURITY_PERMISSIONS`      | :white_check_mark: | :white_check_mark: | :white_check_mark: | :no_entry_sign:
`FEATURE_SERIALIZATION`             | :white_check_mark: | :white_check_mark: | :white_check_mark: | :no_entry_sign:
`FEATURE_SMTP`                      | :white_check_mark: | :white_check_mark: | :white_check_mark: | :no_entry_sign:
`FEATURE_SYSTEM_CONFIGURATION`      | :white_check_mark: | :white_check_mark: | :white_check_mark: | :no_entry_sign:
`FEATURE_TARGETEXCEPTION`           | :white_check_mark: | :white_check_mark: | :white_check_mark: | :no_entry_sign:
`FEATURE_TEST_COM`                  | :white_check_mark: | :white_check_mark: | :white_check_mark: | :no_entry_sign:
`FEATURE_TEST_SERILOGINTEGRATION`   | :no_entry_sign:    | :no_entry_sign:    | :white_check_mark: | :white_check_mark:
---                                 |                    |                    |                    | 
`DOTNET35`                          | :white_check_mark: | :no_entry_sign:    | :no_entry_sign:    | :no_entry_sign:
`DOTNET40`                          | :no_entry_sign:    | :white_check_mark: | :white_check_mark: | :no_entry_sign:
`DOTNET45`                          | :no_entry_sign:    | :no_entry_sign:    | :white_check_mark: | :no_entry_sign:

* `FEATURE_APPDOMAIN` - enables support for features that make use of an AppDomain in the host.
* `FEATURE_APTCA` - marks the assembly with the AllowPartiallyTrustedCallersAttribute.
* `FEATURE_ASSEMBLYBUILDER_SAVE` - enabled support for saving the dynamically generated proxy assembly.
* `FEATURE_BINDINGLIST` - enables support features that make use of System.ComponentModel.BindingList.
* `FEATURE_DICTIONARYADAPTER_XML` - enable DictionaryAdapter Xml features.
* `FEATURE_EMIT_CUSTOMMODIFIERS` - enables emitting optional and required custom modifiers defined on parameters including return parameters. It seems like a defect in corefx not to expose these methods because they are still implemented.
* `FEATURE_EVENTLOG` - provides a diagnostics logger using the Windows Event Log.
* `FEATURE_GAC` - enables support for obtaining assemblies using an assembly long form name.
* `FEATURE_GET_REFERENCED_ASSEMBLIES` - enables code that takes advantage of System.Reflection.Assembly.GetReferencedAssemblies().
* `FEATURE_IDATAERRORINFO` - enables code that depends on System.ComponentModel.IDataErrorInfo.
* `FEATURE_ISUPPORTINITIALIZE` - enables support for features that make use of System.ComponentModel.ISupportInitialize.
* `FEATURE_LEGACY_REFLECTION_API` - provides a shim for .NET 3.5/4.0 that emulates the `TypeInfo` API available in .NET 4.5+ and .NET Core.
* `FEATURE_LISTSORT` - enables support for features that make use of System.ComponentModel.ListSortDescription.
* `FEATURE_NETCORE_REFLECTION_API` - provides shims to implement missing functionality in .NET Core that has no alternatives.
* `FEATURE_REMOTING` - supports remoting on various types including inheriting from MarshalByRefObject.
* `FEATURE_SECURITY_PERMISSIONS` - enables the use of CAS and Security[Critical|SafeCritical|Transparent].
* `FEATURE_SERIALIZATION` - enables support for serialization of dynamic proxies and other types.
* `FEATURE_SMTP` - provides the email sender abstraction and implementation.
* `FEATURE_SYSTEM_CONFIGURATION` - enables features that use System.Configuration and the ConfigurationManager.
* `FEATURE_TARGETEXCEPTION` - enabled catching a `TargetException`. `System.Reflection.TargetException` is implemented by .NET Core but not exposed by corefx.
* `FEATURE_TEST_COM` - enables COM Interop tests.
* `FEATURE_TEST_SERILOGINTEGRATION` - enables Serilog intergration tests.

The `__MonoCS__` symbol is used only in unit tests when compiled on Mono to work around Mono defects and limitations,
however we are moving away from platform specific symbols as much as possible.
