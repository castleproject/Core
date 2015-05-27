# Use *proxy generation hooks* and *interceptor selectors* for fine grained control

Does your interceptor look like this?

```csharp
public void Intercept(IInvocation invocation)
{
    if (invocation.TargetType != typeof(Foo))
    {
        invocation.Proceed();
        return;
    }

    if (invocation.Method.Name != "Bar")
    {
        invocation.Proceed();
        return;
    }

    if (invocation.Method.GetParameters().Length != 3)
    {
        invocation.Proceed();
        return;
    }

    DoSomeActualWork(invocation);
}
```

## Solution

If they do this often means you're doing something wrong. Move the decisions to `IProxyGenerationHook` and `IInterceptorSelector`.

* Do I ever want to intercept this method? If the answer is no, use proxy generation hook to filter it out of methods to proxy.

> Notice that due to bug in DynamicProxy 2.1, if you choose not to proxy method on interface proxy, you will get an exception. Workaround for this is to say you want to intercept the method, and then use interceptor selector to return no interceptors for the method. This bug is fixed in DynamicProxy 2.2

* If I do want to intercept this method, which interceptors do I want to use? Do I need all of them? Do I need just a single one? Use interceptor selector to control this.

## When **not to** do this

On the other hand, remember that as every feature this one is also a double edged sword. Too liberal use of proxy generation hooks and interceptor selectors may greatly decrease efficiency of proxy type caching, which may hurt your performance. As always think how much control you need and what the implications on caching will be. Sometimes single if on top of your interceptor is lesser evil than increasing number of proxies required tenfold. As always – use the profiler in scenarios that mimic your production scenarios as closely as possible to check which option is the best for you.

## See also

* [SRP applies to interceptors](dynamicproxy-srp-applies-to-interceptors.md)