# Castle Core

<img align="right" src="docs/images/castle-logo.png">

Castle Core provides common Castle Project abstractions including logging services. It also features **Castle DynamicProxy** a lightweight runtime proxy generator, and **Castle DictionaryAdapter**.

See the [documentation](docs/README.md).

## Releases

See the [Releases](https://github.com/castleproject/Core/releases).

## Copyright

Copyright 2004-2015 Castle Project

## License

Castle Core is licensed under the [Apache 2.0](http://opensource.org/licenses/Apache-2.0) license. Refer to license.txt for more information.

## Building

### .NET Framework and Silverlight

```
msbuild /p:Configuration=NET45-Release /t:RunAllTests buildscripts/Build.proj
msbuild /p:Configuration=NET40-Release /t:RunAllTests buildscripts/Build.proj
msbuild /p:Configuration=NET35-Release /t:RunAllTests buildscripts/Build.proj
msbuild /p:Configuration=SL50-Release /t:RunAllTests buildscripts/Build.proj
msbuild /p:Configuration=SL40-Release /t:RunAllTests buildscripts/Build.proj
```

### Mono

```
xbuild /p:Configuration=NET45-Release /t:RunAllTests buildscripts/Build.proj
```

### Conditional Compilation Symbols

**TODO:** Retire these `PHYSICALASSEMBLY`, `CLIENTPROFILE`, `DOTNET`.

Symbol                  | NET35              | NET40              | NET45              | SL40               | SL50
----------------------- | ------------------ | ------------------ | ------------------ | ------------------ | ------------------
`FEATURE_SERIALIZATION` | :white_check_mark: | :white_check_mark: | :white_check_mark: | :no_entry_sign:    | :no_entry_sign:
`DOTNET35`              | :white_check_mark: | :no_entry_sign:    | :no_entry_sign:    | :no_entry_sign:    | :no_entry_sign:
`DOTNET40`              | :no_entry_sign:    | :white_check_mark: | :white_check_mark: | :no_entry_sign:    | :no_entry_sign:
`DOTNET45`              | :no_entry_sign:    | :no_entry_sign:    | :white_check_mark: | :no_entry_sign:    | :no_entry_sign:
`SILVERLIGHT`           | :no_entry_sign:    | :no_entry_sign:    | :no_entry_sign:    | :white_check_mark: | :white_check_mark:
`SL4`                   | :no_entry_sign:    | :no_entry_sign:    | :no_entry_sign:    | :white_check_mark: | :no_entry_sign:

The `__MonoCS__` symbol is used only in unit tests when compiled on Mono to work around Mono defects and non-Windows differences, however we are trying to move away from platform specific symbols as much as possible.