# Castle DynamicProxy

DynamicProxy generates proxies for your objects that you can use to transparently add or alter behavior to them, provide pre/post processing and many other things. Following are just a few better known and popular usages of DynamicProxy:

* [Castle Windsor](http://www.castleproject.org/projects/windsor/) uses proxies to enable its interception capabilities and for typed factories
* [Moq](https://github.com/moq/moq4) uses it to provide "the most popular and friendly mocking framework for .NET"
* [NSubstitute](http://nsubstitute.github.io/) uses it to provide "a friendly substitute for .NET mocking frameworks"
* [FakeItEasy](http://fakeiteasy.github.io/) uses it to provide "the easy mocking library for .NET"
* [Rhino Mocks](https://www.hibernatingrhinos.com/oss/rhino-mocks) uses it to provide "a dynamic mock object framework for the .NET platform"
* [NHibernate](http://nhibernate.info/) uses it to provide lazy loading capabilities (pre-v4.0)
* [Entity Framework Core](https://github.com/aspnet/EntityFrameworkCore) uses it in its package [Microsoft.EntityFrameworkCore.Proxies](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Proxies) to provide lazy-loading proxies

If you're new to DynamicProxy you can read a [quick introduction](dynamicproxy-introduction.md), browse through description of the core types in the library, or go into more advanced, in detail discussion:
* [Kinds of proxy objects](dynamicproxy-kinds-of-proxy-objects.md)
* [Leaking this](dynamicproxy-leaking-this.md)
* [Make proxy generation hooks purely functional](dynamicproxy-generation-hook-pure-function.md)
* [Overriding Equals/GetHashCode on proxy generation hook](dynamicproxy-generation-hook-override-equals-gethashcode.md)
* [Make your supporting classes serializable](dynamicproxy-serializable-types.md)
* [Use proxy generation hooks and interceptor selectors for fine grained control](dynamicproxy-fine-grained-control.md)
* [SRP applies to interceptors](dynamicproxy-srp-applies-to-interceptors.md)
* [Behavior of by-reference parameters during interception](dynamicproxy-by-ref-parameters.md)
* [Optional parameter value limitations](dynamicproxy-optional-parameter-value-limitations.md)

:information_source: **Where is `Castle.DynamicProxy.dll`?:** DynamicProxy used to live in its own assembly. As part of changes in version 2.5 it was merged into `Castle.Core.dll` and that's where you'll find it.

:warning: **Use of a single ProxyGenerator's instance:** If you have a long running process (web site, windows service, etc.) and you have to create many dynamic proxies, you should make sure to reuse the same ProxyGenerator instance.  If not, be aware that you will then bypass the caching mechanism.  Side effects are high CPU usage and constant increase in memory consumption.

## See also

* [Castle DictionaryAdapter](dictionaryadapter.md)
