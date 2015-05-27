# Leaking `this`

## Problem

Consider this simple interface/class pair:

```csharp
public interface IFoo
{
    IFoo Bar();
}

public class Foo : IFoo
{
    public IFoo Bar()
    {
        return this;
    }
}
```

Now, let's say we create a proxy for `IFoo` with target and use it like this:

```csharp
var foo = GetFoo(); // returns proxy
var bar = foo.Bar();
bar.Bar();
```

Can you see the bug here? The second call is performed not on a proxy but on a target object itself! Our proxy is leaking its target.

This issue obviously does not affect class proxies (since in that case proxy and target are the same object). Why doesn't Dynamic Proxy handle this scenario on its own? Because there's no general easy way to handle this. The example I showed is the most trivial one, but proxied object can leak this in a myriad of different ways. It can leak it as a property of returned object, it can leak it as sender argument of raised event, it can assign this to some global variable, it can pass itself to a method on one of its own arguments etc. Dynamic Proxy can't predict any of these, nor should it.

## Solution

In some of these cases there is often not much you can do about it, and its good to know that problem like this exist, and understand its consequences. In other cases though, fixing the issue is very simple indeed.

```csharp
public class LeakingThisInterceptor:IInterceptor
{
    public void Intercept(IInvocation invocation)
    {
        invocation.Proceed();
        if (invocation.ReturnValue == invocation.InvocationTarget)
        {
            invocation.ReturnValue = invocation.Proxy;
        }
    }
}
```

You add an interceptor (put it as last one in the interceptors pipeline), that switches the leaking target back to proxy instance. It's as simple as that. Notice that this interceptor is targeted specifically at the scenario from our example above (target leaking via return value). For each case you will need a dedicated interceptor.