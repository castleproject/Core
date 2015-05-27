# Kinds of Proxy Objects

Out of the box, the DynamicProxy provides several kinds of proxy objects that you can use. They fall into two broad categories:

## Inheritance-based

Inheritance-based proxies are created by inheriting a proxy class. The proxy intercepts calls to virtual members of the class and forwards them to base implementation. In this case, the proxy and proxied object are one. This also means you can't create inheritance based proxy for a pre-existing object. There's one inheritance-based proxy kind in DynamicProxy.

* Class proxy - creates an inheritance-based proxy for a class. Only virtual members of the class can be intercepted.

## Composition-based

Composition-based proxy is a new object that inherits from proxied class/implements proxied interface and (optionally) forwards intercepted calls to the target object. DynamicProxy exposes the following composition-based proxy types:

* Class proxy with target - this proxy kind targets classes. It is not a perfect proxy and if the class has non-virtual methods or public fields these can't be intercepted giving users of the proxy inconsistent view of the state of the object. Because of that, it should be used with caution.

* Interface proxy without target - this proxy kind targets interfaces. It does not expect target object to be supplied. Instead, interceptors are expected to provide implementation for all members of the proxy.

* Interface proxy with target - as its name implies wraps objects implementing selected interfaces forwarding calls to these interfaces to the target object.

* Interface proxy with target interface - this kind of proxy is kind of a hybrid of two other interface proxy kinds. It allows, but not requires target object to be supplied. It also allows the target to be swapped during the lifetime of the proxy. It is not tied to one type of the proxy target so one proxy type can be reused for different target types as long as they implement the target interface.

## External resources

* [Tutorial on DynamicProxy discussing with examples all kinds of proxies](http://kozmic.pl/dynamic-proxy-tutorial/)
* [Castle Dynamic Proxy not intercepting method calls when invoked from within the class](http://stackoverflow.com/questions/6633914/castle-dynamic-proxy-not-intercepting-method-calls-when-invoked-from-within-the/)
* [Why won't DynamicProxy's interceptor get called for *each* virtual method call?](http://stackoverflow.com/questions/2153019/why-wont-dynamicproxys-interceptor-get-called-for-each-virtual-method-call)