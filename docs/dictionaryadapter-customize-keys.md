# Castle DictionaryAdapter - Customizing Adapter Keys

By default DictionaryAdapter will use very simple strategy when inserting/retrieving data to/from dictionary via adapter - property name becomes the key.

So doing:

```
adapter.Name = "Stefan";
```

is identical to doing

```csharp
dictionary["Name"] = "Stefan";
```

Same with reading

```csharp
var name = adapter.Name;
```

is identical to doing

```csharp
var name = (string)dictionary["Name"];
```

:information_source: **A word about conversion:** Well that's not entirely true. DictionaryAdapter uses a more sophisticated mechanism for conversion than just plain casting.

This is a fine default, but the adapter lets you customize that behavior using attributes.

For the examples below, lets assume the setup is as follows:

```csharp
var dictionary = new Hashtable();
var adapter = new DictionaryAdapterFactory.GetAdapter<IPerson>(dictionary);
```

## Standard Attributes

Out of the box DictionaryAdapter provides set of attributes that should suffice for majority of cases.

### `Key` Attribute

If you want the property name and dictionary key to be totally unrelated you can override the default name using `KeyAttribute`.

```csharp
public interface IPerson
{
   [Key("PersonId")]
   string Name {get; set;}
}
```

Now:

```csharp
adapter.Name == dictionary["PersonId"];
```

### `KeyPrefix` Attribute

If you would like to prefix all keys in dictionary with some string use `KeyPrefixAttribute`.

```csharp
[KeyPrefix("Person")]
public interface IPerson
{
   string Name {get; set;}
}
```

Now:

```csharp
adapter.Name == dictionary["PersonName"];
```

If we had more properties on the interface, they'd all get prefixed.

### `TypeKeyPrefix` Attribute

If you would like to prefix all keys in dictionary with full name of the interface use `TypeKeyPrefixAttribute`. This is especially useful when you have multiple adapters over common dictionary and you want to avoid one overriding another.

```csharp
[TypeKeyPrefix]
public interface IPerson
{
   string Name {get; set;}
}
```

Now:

```csharp
adapter.Name == dictionary["Acme.Crm.IPerson#Name"];
```

If we had more properties on the interface, they'd all get prefixed.

### `KeySubstitution` Attribute

If you would like to replace certain characters in your key use `TypeKeyPrefixAttribute`. This is especially useful when you want to use certain characters in your key, that are not legal in C# names. You can set the attribute at interface level, or at property level.

```csharp
public interface IPerson
{
   [KeySubstitution("_",".")]
   string Full_Name {get; set;}
}
```

Now:

```cs`harp
adapter.Full_Name == dictionary["Full.Name"];
```

## Creating Custom Attributes

Custom attributes need to implement two interfaces:

* `IDictionaryBehavior` - Standard base type for all DictionaryAdapter attributes. Usually you'll inherit base `DictionaryBehaviorAttribute` class and forget about it.
* `IDictionaryKeyBuilder` - that's the contract for customizing the key.

### Example - `KeyPostfix` Attribute

As an example let's assume we want to create attribute that adds common postfix to the property keys, so that we can use it like follows:

```csharp
public interface IPerson
{
   [KeyPostfix("Person")]
   string Name {get; set;}
}
```

and get:

```csharp
adapter.Name == dictionary["NamePerson"];
```

Simple implementation might look like this:

```csharp
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class KeyPostfixAttribute : DictionaryBehaviorAttribute, IDictionaryKeyBuilder
{
	private String postfix;

	public KeyPrefixAttribute(string keyPrefix)
	{
		this.postfix = keyPrefix;
	}

	String IDictionaryKeyBuilder.GetKey(IDictionaryAdapter dictionaryAdapter, String key, PropertyDescriptor property)
	{
		return key + postfix;
	}
}
```