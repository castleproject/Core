# SRP applies to interceptors

SRP stands for Single Responsibility Principle, which means that a class should do just one thing. Many people seem to forget about it when it comes to interceptors. They create one monstrous interceptor class that tries to do all the things they need from Dynamic Proxy – logging, security checking, parameter verification, augmenting target objects with behavior and many more.

Remember that DynamicProxy lets you have many interceptors per method call. Use this ability to split behavior between interceptors. You may end up with some general purpose interceptors for things like logging that you use for each intercepted method on each class. As long as all it does is logging – that's OK.

You may end up with some interceptors that are used for methods on just some classes, like classes inheriting from common base class. As long as these interceptors do just one thing – that's fine.

You may end up with some interceptors that exist solely for the purpose of intercepting just a single method on specific class or interface. That also is fine. Use interceptor selectors to match interceptors to their respective targets, and don't be afraid to have multiple interceptors per method.

## See also

* [Use proxy generation hooks and interceptor selectors for fine grained control](dynamicproxy-fine-grained-control.md)