using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Castle.Components.DictionaryAdapter.Xml
{
	public interface IXmlAccessor
	{
		Type ClrType { get; }
		IXmlKnownTypeMap KnownTypes { get; }
		XmlTypeSerializer Serializer { get; }

		IXmlCollectionAccessor GetCollectionAccessor(Type itemType);
	}
}
