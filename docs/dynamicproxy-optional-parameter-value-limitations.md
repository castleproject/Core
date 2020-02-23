# Optional parameter value limitations

DynamicProxy uses `System.Reflection` and `System.Reflection.Emit` to create proxy types. Due to several bugs and limitations in these facilities (both on .NET and Mono), it is not possible in every single case to faithfully reproduce default values of optional parameters in the dynamically generated proxy types.

This usually doesn't matter much in practice, but it may become a problem for frameworks or libraries (such as Dependency Injection containers) that reflect over the generated proxy types.


## When your code runs on Mono

On Mono (up to and including at least version 5.16), DynamicProxy may not be able to correctly reproduce default parameter values in the proxy type for...

* **Optional parameters of type `DateTime?` or `decimal?`.** Reflection will likely report (via `ParameterInfo.[Raw]DefaultValue`) a default value of `null` (up until Mono 5.10) or `Missing.Value` (on 5.16) for such parameters, regardless of the actual default parameter value.

   You can only find out the correct default value by double-checking the default value in the original method of the proxied type.

   The underlying causes have been documented (for Mono 5.10) in [mono/mono#8504](https://github.com/mono/mono/issues/8504) and [mono/mono#8597](https://github.com/mono/mono/issues/8597), and (for Mono 5.16) in [mono/mono#11303](https://github.com/mono/mono/issues/11303).


## When your code runs on the .NET Framework or on .NET Core

The .NET Framework (up to and including at least version 4.7.1) and .NET Core (up to and including at least version 2.1) are affected by several bugs or limitations regarding default parameter values. DynamicProxy may not be able to correctly reproduce default parameter values in the proxy type for...

* **Optional parameters of any nullable type `Nullable<T>`.** On the .NET Framework 3.5 only, reflection will likely report (via `ParameterInfo.[Raw]DefaultValue`) a default value of `Missing.Value` for such parameters.

   There is no easy way to quickly guess what the correct default value might be. Consider upgrading to the .NET Framework 4 or later, or double-check the default value in the original method of the proxied type.

* **Optional parameters of some `struct` type `SomeStruct` having a default value of `default(SomeStruct)`.** If reflection reports (via `ParameterInfo.[Raw]DefaultValue`) a default value of `Missing.Value` for such parameters, you may safely assume that the *correct* default value is `default(SomeStruct)`.

   Note that if reflection reports a default value of `null` in such cases, this is not an error, but normal `System.Reflection` behavior that is to be expected. In this case, you may also safely assume `default(SomeStruct)` to be the correct default value.

   For .NET Core, the underlying cause of this problem has been documented in [dotnet/corefx#26164](https://github.com/dotnet/corefx/issues/26164).

* **Optional parameters of some nullable `enum` type `SomeEnum?` having a non-`null` default value.** If reflection reports (via `ParameterInfo.[Raw]DefaultValue`) a default value of `Missing.Value` for such parameters, the only thing you may safely assume is that the actual default value is not `null`.

   You can only find out the correct default value by double-checking the default value in the original method of the proxied type.

   For .NET Core, the underlying cause of this problem has been documented in [dotnet/coreclr#17893](https://github.com/dotnet/coreclr/issues/17893).

* **Optional parameters of a generic parameter type instantiated as some `enum` type `SomeEnum`.** For example, given a generic type `C<T>` and a method `void M(T arg = default(T))`, if you proxy the closed generic type `C<SomeEnum>`, reflection might then report (via `ParameterInfo.[Raw]DefaultValue`) a default value of `Missing.Value` for the proxied `arg` parameter. If so, you may safely assume that the actual default value is `default(SomeEnum)`.

   Note that if reflection reports a default value of `null` in such cases, this is not an error, but normal `System.Reflection` behavior that is to be expected. In this case, you may also safely assume `default(SomeEnum)` to be the correct default value.

   For .NET Core, the underlying cause of this problem has been documented in [dotnet/coreclr#29570](https://github.com/dotnet/corefx/issues/29570).
