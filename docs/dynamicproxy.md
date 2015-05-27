# Castle DynamicProxy

DynamicProxy generates proxies for your objects that you can use to transparently add or alter behavior to them, provide pre/post processing and many other things. Castle Windsor uses proxies to enable its interception capabilities, or for Typed Factories. [NHibernate](http://nhforge.org) uses it to provide lazy loading capabilities. [Moq](http://moq.me) and [Rhino Mocks](http://www.ayende.com/projects/rhino-mocks.aspx) both use it to provide their mocking capabilities. These are just a few better known and popular usages of the framework.

* If you're new to DynamicProxy you can read a [quick introduction](dynamicproxy-introduction.md).
* Browse through description of the core types in the library.
* Go into more advanced, in detail discussion:
  * [Kinds of proxy objects](dynamicproxy-kinds-of-proxy-objects.md)
  * [Leaking this](dynamicproxy-leaking-this.md)
  * [Make proxy generation hooks purely functional](dynamicproxy-generation-hook-pure-function.md)
  * [Overriding Equals/GetHashCode on proxy generation hook](dynamicproxy-generation-hook-override-equals-gethashcode.md)
  * [Make your supporting classes serializable](dynamicproxy-serializable-types.md)
  * [Use proxy generation hooks and interceptor selectors for fine grained control](dynamicproxy-fine-grained-control.md)
  * [SRP applies to interceptors](dynamicproxy-srp-applies-to-interceptors.md)

:information_source: **Where is `Castle.DynamicProxy.dll`?:** DynamicProxy used to live in its own assembly. As part of changes in version 2.5 it was merged into `Castle.Core.dll` and that's where you'll find it.

:warning: **Use of a single ProxyGenerator's instance:** If you have a long running process (web site, windows service, etc.) and you have to create many dynamic proxies, you should make sure to reuse the same ProxyGenerator instance.  If not, be aware that you will then bypass the caching mechanism.  Side effects are high CPU usage and constant increase in memory consumption.

## See also

* [Castle DictionaryAdapter](dictionaryadapter.md)