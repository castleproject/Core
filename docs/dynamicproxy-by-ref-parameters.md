# Behavior of by-reference parameters during interception

DynamicProxy has support for by-reference parameters (that is, those having a `ref` or `out` modifier in C#, or a `ByRef` modifier in Visual Basic). However, those parameters work *slightly* differently during an intercepted invocation than they would during a regular (non-intercepted) method call. In most use cases, you will probably not notice this difference at all; but when you do, it can be helpful to understand why it exists.

**During a regular method invocation**, a by-reference parameter is a perfect alias for the variable being passed by the caller: Changing the value of a `ref` parameter, for instance, instantly changes the aliased variable.

**During a DynamicProxy method interception**, on the other hand, changes to a `ref` parameter (which you would perform using the `Arguments` property or `SetArgumentValue` method of the `IInvocation` instance) are not immediately reflected in the aliased variable. The change in the aliased variable will only become visible upon completion of the interception. This is because DynamicProxy essentially "buffers" all invocation arguments in a separate storage location (`IInvocation.Arguments`) for the whole duration of a method interception.

* When interception begins, DynamicProxy copies all arguments provided by the caller over into the `Arguments` array of the `IInvocation` object. This copying happens "by value". Those `Arguments` indices representing a by-reference parameter are *not* aliased to a caller variable; they are distinct storage locations. Therefore, changing `Arguments` is not immediately propagated.

* When interception ends, DynamicProxy will copy the final values from `Arguments` back to the original `ref` and `out` parameters. This is the moment when any changes become visible in the aliased variables of the caller.

In short, modifying a by-reference parameter during interception will be reflected in the aliased variable *eventually, but not immediately*.

## Demonstration

Let's look at this difference in behavior in action. The following code examples will be based on this interface:

```csharp
public interface IService
{
    void Execute(ref int n);
}
```

### How by-reference parameters work in plain C# (or Visual Basic)

As stated above, by-reference parameters are normally perfect aliases for other variables, and changes to them should become immediately observable on those aliased variables.

We can see this behavior using the following implementation of `IService`:

```csharp
sealed class Service : IService
{
    private ExecuteDelegate execute;

    public Service(ExecuteDelegate execute) => this.execute = execute;

    public void Execute(ref int n) => this.execute?.Invoke(ref n);
}

delegate void ExecuteDelegate(ref int n);
```

and then using and invoking `IService.Execute` as follows:

```csharp
int n = 0;

var service = new Service(
    execute: (ref int alias) =>
    {
        Console.WriteLine(n);  // => 0
        alias = 42;
        Console.WriteLine(n);  // => 42
    });

Console.WriteLine(n);          // => 0
service.Execute(ref n);
Console.WriteLine(n);          // => 42
```

Note that inside the lambda, we change the `alias` parameter but can observe the change to `n` (to which it is aliased) immediately. 

### How by-reference parameters work during a DynamicProxy interception

As explained above, changes to `ref` parameters will be observable in the aliased variable only *after, but not during* interception. Let's see an example. Instead of implementing `IService` ourselves, we will let DynamicProxy create an instance. So we will need an interceptor:

```csharp
sealed class Interceptor : IInterceptor
{
    private Action<IInvocation> intercept;

    public Interceptor(Action<IInvocation> intercept) => this.intercept = intercept;

    public void Intercept(IInvocation invocation) => this.intercept?.Invoke(invocation);
}
```

Note the similarity to the above example's implementation of `IService`. The basic idea here is the same: We want to be able to define what should happen during method invocation inside a lambda that has access to the aliased variable:

```csharp
int n = 0;

var service = new ProxyGenerator().CreateInterfaceProxyWithoutTarget<IService>(new Interceptor(
    intercept: invocation =>
    {
        Console.WriteLine(n);  // => 0
        invocation.SetArgumentValue(0, 42);
        Console.WriteLine(n);  // => 0 !!!
    }));

Console.WriteLine(n);          // => 0
service.Execute(ref n);
Console.WriteLine(n);          // => 42
```

Note that this time, changing the argument value during invocation does not immediately affect the aliased variable `n`,
even though it is eventually updated.
