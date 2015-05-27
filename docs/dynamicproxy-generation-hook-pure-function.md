# Make proxy generation hooks purely functional

Pure function, is a function that for given set of inputs always returns the same output. In case of proxy generation hook, it means that two equal (as specified by overridden `Equals`/`GetHashCode` methods) proxy generation hooks will for given type to proxy return the same values from their methods, and when asked again about the same type will again return the same values/throw the same exceptions.

This is a major assumption that DynamicProxy makes, and that's what makes the caching mechanism work. If proxy generation hook is equal to the one already used to generate a proxy type, DynamicProxy will assume it would return the same values as the other one, which would result in identical proxy type, so it cuts through the generation process and returns the existing proxy type.

## See also

* [Overriding Equals/GetHashCode on proxy generation hook](dynamicproxy-generation-hook-override-equals-gethashcode.md)