# Support for byref-like (`ref struct`) parameters and return types

Starting with version 6.0.0 of the library, and when targeting .NET 8 or later, DynamicProxy has comprehensive support for byref-like types.


## What are byref-like types?

Byref-like types &ndash; also known as `ref struct` types in C# &ndash; are a special category of types that by definition live exclusively on the evaluation stack. Therefore, they can never be found on the heap, or be parts of heap-allocated objects. This implies that unlike other value types, they cannot be boxed (converted to `object`).


## How does DynamicProxy place `ref struct` values into an `IInvocation`?

The impossibility of converting byref-like values to `object` poses a fundamental problem for DynamicProxy, which represents all of an intercepted method's arguments as well as the intended return value as `object`s in an `IInvocation` instance: arguments are placed in the `object[]`-typed `IInvocation.Arguments` property, and the intended return value can be set via the `object`-typed `IInvocation.ReturnValue` property. So how can `ref struct`-typed argument values possibly appear in `IInvocation`?

The answer is that they cannot. DynamicProxy substitutes `ref struct` argument values with values of different, boxable types:

 * `System.Span<T>` values are replaced with instances of `Castle.DynamicProxy.SpanReference<T>`.
 * `System.ReadOnlySpan<T>` values are replaced with instances of `Castle.DynamicProxy.ReadOnlySpanReference<T>`.
 * Values of any other `ref struct` type `TByRefLike` are replaced with instances of `Castle.DynamicProxy.ByRefLikeReference<TByRefLike>` (for .NET 9+) or with `Castle.DynamicProxy.ByRefLikeReference` (for .NET 8).

Here is a diagram showing these types' hierarchy:
```
   (not for direct use)                 (only available on .NET 9+)
+----------------------+        +----------------------------------+          +--------------------------------+
|  ByRefLikeReference  | <----- |  ByRefLikeReference<TByRefLike>  | <------- |    ReadOnlySpanReference<T>    |
+----------------------+        +==================================+    \     +================================+
                                |  +Value: ref TByRefLike          |     \    |  +Value: ref ReadOnlySpan<T>   |
                                +----------------------------------+      \   +-------------------------------++
                                                                           \
                                                                            \      +------------------------+
                                                                             \     |    SpanReference<T>    |
                                                                              ---- +========================+
                                                                                   |  +Value: ref Span<T>   |
                                                                                   +------------------------+
```

These types do not contain the actual byref-like values, but (like their names imply) they hold a "reference" that they can resolve to the values. With the exception of the non-generic `ByRefLikeReference` substitute type (which is meant for use by the DynamicProxy runtime, not by user code!), all of these expose a `ref`-returning `Value` property which can be used to:

 * read the actual byref-like argument or return value; or,
 * for `ref` and `out` parameters, update the parameter.


## Basic usage example


### Reading (and updating) a byref-like parameter

```csharp
public interface TypeToBeProxied
{
    void Method(ref Span<int> arg);
}

var proxy = proxyGenerator.CreateInterfaceProxyWithoutTarget<TypeToBeProxied>(new MethodInterceptor());

class MethodInterceptor : IInterceptor
{
    public void Intercept(IInvocation invocation)
    {
        var argRef = (SpanReference<int>)invocation.Arguments[0];

        Span<int> arg = argRef.Value; // read the argument value
        argRef.Value = arg[0..^1]; // update the parameter (only makes sense for `ref` and `out` parameters)
    }
}
```


### Returning a byref-like value

```csharp
public interface TypeToBeProxied
{
    ReadOnlySpan<char> Method();
}

var proxy = proxyGenerator.CreateInterfaceProxyWithoutTarget<TypeToBeProxied>(new MethodInterceptor());

class MethodInterceptor : IInterceptor
{
    public void Intercept(IInvocation invocation)
    {
        var returnValueRef = (ReadOnlySpanReference<int>)invocation.ReturnValue;

        var returnValue = "Hello there!".AsSpan();
        returnValueRef.Value = returnValue;
    }
}
```


## Rules for safe & correct usage

 1. ✅ The only permissible interaction with values of the above-mentioned substitute types &ndash; `SpanReference<T>`, `ByRefLikeReference<TByRefLike>`, etc. &ndash; is reading and writing their `Value` property.

 2. 🚫 The substitutes may only be accessed while the intercepted method is running. Once it returns, the substitute values become invalid, and any further attempts at using them in any way will cause immediate `AccessViolationException`s. (The reason for this is that the substitutes reference storage locations in the intercepted method's stack frame. Once the method stops executing, its stack frame is popped off the evaluation stack, which effectively ends the method's arguments' lifetime.)

    - DynamicProxy tries to prevent you from making this mistake by actively erasing the substitute values from the `IInvocation` instance once the intercepted method returns to the caller.

    - Note also that the substitute values cannot survive an async `await` or `yield return` boundary for the same reason. (This is not unlike the C# language rule which forbids the use of byref-like parameters in `async` methods.)

    - A final consequence of this is that `ref struct` arguments will be unavailable in an interception pipeline restarted by `IInvocationProceedInfo` / `invocation.CaptureProceedInfo`.

 3. 🚫 It is very strongly recommended that values of the substitute types not be copied anywhere else. DynamicProxy places them in specific spots inside an `IInvocation` instance, and that is precisely where they should stay. Don't copy them into variables for later use, or from one `IInvocation.Arguments` array position to another, etc.

    - DynamicProxy performs some checks against this practice, and if any such attempt is detected, an `AccessViolationException` gets thrown.
