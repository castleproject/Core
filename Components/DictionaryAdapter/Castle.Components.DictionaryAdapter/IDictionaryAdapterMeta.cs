using System;
using System.Collections;
using System.Collections.Generic;

namespace Castle.Components.DictionaryAdapter
{
	public interface IDictionaryAdapterMeta
	{
		IDictionary Dictionary { get; }

		IDictionary<String, PropertyDescriptor> Properties { get; }

		void FetchProperties();
	}
}
