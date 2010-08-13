namespace Castle.Components.DictionaryAdapter
{
	using System;
	using System.Collections.Generic;

	public class GenericDictionaryAdapter<TValue> : AbstractDictionaryAdapter
	{
		private readonly IDictionary<string, TValue> _dictionary;

		public GenericDictionaryAdapter(IDictionary<string, TValue> dictionary)
		{
			_dictionary = dictionary;
		}

		public override bool IsReadOnly
		{
			get { return _dictionary.IsReadOnly; }
		}

		public override bool Contains(object key)
		{
			return _dictionary.Keys.Contains(GetKey(key));
		}

		public override object this[object key]
		{
			get { return _dictionary[GetKey(key)]; }
			set { _dictionary[GetKey(key)] = (TValue)value; }
		}

		private static string GetKey(object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			return key.ToString();
		}
	}

	public static class GenericDictionaryAdapter
	{
		public static GenericDictionaryAdapter<TValue> ForDictionaryAdapter<TValue>(this IDictionary<string, TValue> dictionary)
		{
			return new GenericDictionaryAdapter<TValue>(dictionary);
		}
	}
}
