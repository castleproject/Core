# Overriding Equals/GetHashCode on proxy generation hook

## Equals/GetHashCode

One of the most common mistakes when it comes to DynamicProxy is not overriding `Equals`/`GetHashCode` methods on proxy generation hooks, which means you're giving up caching and that in turn coupled with bugs in BCL means performance hit (plus increased memory consumption).

## Solution

Solution is very simple, and there's no exceptions to this rule – always override `Equals`/`GetHashCode` methods on all your classes implementing `IProxyGenerationHook`.

## See also

* [Make proxy generation hooks purely functional](dynamicproxy-generation-hook-pure-function.md)