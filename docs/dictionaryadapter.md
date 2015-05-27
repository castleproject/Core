# Castle DictionaryAdapter

## Introduction

DictionaryAdapter is a lightweight tool that on the fly generates strongly typed wrappers on top of `IDictionary` (and its generic brother) type. Not only that, but it also has some other capabilities like support for `INotifyPropertyChanged`, editability, error handling, etc...

It is extremely useful in the context of web applications, as there are many untyped dictionaries in use, such as `Session`, `Form`, `QueryString`, `Context.Items`, and MonoRail's `PropertyBag` and `Flash`. It can also wrap settings from `app.settings`/`web.settings` file.

It can also be used with any other dictionary in any other part of your application.

## Hello World Example

Hello world example of DictionaryAdapter is just few lines of code.

### Using

After you've added a reference to `Castle.Core.dll` you need to import the following namespace:

```csharp
using Castle.Components.DictionaryAdapter;
```

### The Interface

First we just need a plain simple interface:

```csharp
public interface IHelloWorld
{
    string Message { get; }
}
```

### Reading

We can now create an adapter for the interface:

```csharp
var dictionary = new Hashtable();
var factory = new DictionaryAdapterFactory();
var adapter = factory.GetAdapter<IHelloWorld>(dictionary);
dictionary["Message"] = "Hello world!";
Debug.Assert(adapter.Message == "Hello world!");
```

We start with the dictionary we want to wrap with the adapter. In actual applications this would for example be http session.
Then we need something to create the adapter with. Meet `DictionaryAdapterFactory`.

:information_source: **Reuse `DictionaryAdapterFactory`:** The `DictionaryAdapterFactory` is like DynamicProxy's `ProxyGenerator` - you normally wouldn't just create it on the spot each time you need an adapter. You should strive to reuse it as much as possible, so it's a good idea to make it a singleton.

With the factory we create the adapter passing in the dictionary we want to wrap.

That's it - you can now read from the dictionary using the adapter.

### Writing

The interface in the example above had just a getter for its sole property, but if it also had a setter we could write to it as well.

```csharp
adapter.Message = "Hello world!";
Debug.Assert(dictionary["Message"] == "Hello world!");@@
```

### See also

* [Customizing adapter keys](dictionaryadapter-customize-keys.md)