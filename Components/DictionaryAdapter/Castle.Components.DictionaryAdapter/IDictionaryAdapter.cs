using System;
using System.Collections;
using System.Collections.Generic;

namespace Castle.Components.DictionaryAdapter
{
	public interface IDictionaryAdapter
	{
		IDictionary Dictionary { get; }

		IDictionary<String, PropertyDescriptor> Properties { get; }

		IDictionaryAdapterFactory Factory { get; }

		void FetchProperties();
	}
}
