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