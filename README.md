# Castle Core

<img align="right" src="docs/images/castle-logo.png">

Castle Core provides common Castle Project abstractions including logging services. It also features **Castle DynamicProxy** a lightweight runtime proxy generator, and **Castle DictionaryAdapter**.

See the [documentation](docs/README.md).

## Releases

[![NuGet](https://img.shields.io/nuget/v/Castle.Core.svg)](https://www.nuget.org/packages/Castle.Core/)

See the [Releases](https://github.com/castleproject/Core/releases).

Debugging symbols are available in symbol packages in the AppVeyor build artifacts since version 4.1.0. For example, [here are the artifacts for 4.1.0](https://ci.appveyor.com/project/castleproject/core/build/4.1.0/artifacts).

## License

Castle Core is &copy; 2004-2025 Castle Project. It is free software, and may be redistributed under the terms of the [Apache 2.0](http://opensource.org/licenses/Apache-2.0) license.

## Contributing

Browse the [contributing section](https://github.com/castleproject/Home#its-community-driven) of our _Home_ repository to get involved.

## Building

Compilation requires the .NET 10 SDK.

Running the unit tests additionally requires the .NET Framework 4.6.2+ and the .NET 8 and 9 runtimes to be installed. (If you do not have all of those installed, you can run the tests for a specific target framework using `dotnet test -f <framework>`.)

| Platforms       | NuGet Feed |
|-----------------|------------|
| Windows & Linux | [Preview Feed](https://ci.appveyor.com/nuget/core-0mhe40ifodk8)

### On Windows

```
build.cmd
```

### On Linux

```
./build.sh
```

:information_source: **Mono runtime support:** We used to run tests on the Mono 6.0 runtime, but stopped doing so as the project has been deprecated. See the official announcement on [the Mono homepage](https://www.mono-project.com/).

### Conditional Compilation Symbols

The following conditional compilation symbols (vertical) are currently defined for each of the build configurations (horizontal):

Symbol                              | .NET 4.6.2         | .NET Standard 2.0 | .NET 8             | .NET 9
----------------------------------- | ------------------ | ----------------- | ------------------ | ------------------
`FEATURE_APPDOMAIN`                 | :white_check_mark: | :no_entry_sign:   | :no_entry_sign:    | :no_entry_sign:
`FEATURE_ASSEMBLYBUILDER_SAVE`      | :white_check_mark: | :no_entry_sign:   | :no_entry_sign:    | :no_entry_sign:
`FEATURE_BYREFLIKE`                 | :no_entry_sign:    | :no_entry_sign:   | :white_check_mark: | :white_check_mark:
`FEATURE_SERIALIZATION`             | :white_check_mark: | :no_entry_sign:   | :no_entry_sign:    | :no_entry_sign:
`FEATURE_SYSTEM_CONFIGURATION`      | :white_check_mark: | :no_entry_sign:   | :no_entry_sign:    | :no_entry_sign:

* `FEATURE_APPDOMAIN` - enables support for features that make use of an AppDomain in the host.
* `FEATURE_ASSEMBLYBUILDER_SAVE` - enabled support for saving the dynamically generated proxy assembly.
* `FEATURE_BYREFLIKE` - enables support for by-ref-like (`ref struct`) types such as `Span<T>` and `ReadOnlySpan<T>`.
* `FEATURE_SERIALIZATION` - enables support for serialization of dynamic proxies and other types.
* `FEATURE_SYSTEM_CONFIGURATION` - enables features that use System.Configuration and the ConfigurationManager.
