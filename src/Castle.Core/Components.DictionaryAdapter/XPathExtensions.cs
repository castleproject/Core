// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.Components.DictionaryAdapter
{
#if !SILVERLIGHT
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Xml;
	using System.Xml.XPath;

	public static class XPathExtensions
	{
		private const string XmlMetaKey = "XmlMeta";

		public static XmlMetadata GetXmlMeta(this IDictionaryAdapter dictionaryAdapter)
		{
			return GetXmlMeta(dictionaryAdapter, null);
		}

		public static XmlMetadata GetXmlMeta(this IDictionaryAdapter dictionaryAdapter, Type otherType)
		{
			if (otherType == null || otherType.IsInterface)
			{
				var meta = GetDictionaryMeta(dictionaryAdapter, otherType);
				return (XmlMetadata)meta.ExtendedProperties[XmlMetaKey];
			}
			return null;
		}

		public static Type GetXmlSubclass(this IDictionaryAdapter dictionaryAdapter, XmlQualifiedName xmlType, Type otherType)
		{
			if (xmlType == null)
			{
				return null;
			}
			var xmlIncludes = dictionaryAdapter.GetXmlMeta(otherType).XmlIncludes;
			if (xmlIncludes != null)
			{
				var subClass = from xmlInclude in xmlIncludes
				               let xmlIncludeType = dictionaryAdapter.GetXmlMeta(xmlInclude).XmlType
				               where xmlIncludeType.TypeName == xmlType.Name &&
				                     xmlIncludeType.Namespace == xmlType.Namespace
				               select xmlInclude;
				return subClass.FirstOrDefault();
			}
			return null;
		}

		internal static XmlMetadata GetXmlMeta(this DictionaryAdapterMeta dictionaryAdapterMeta)
		{
			return (XmlMetadata)dictionaryAdapterMeta.ExtendedProperties[XmlMetaKey];
		}

		internal static void SetXmlMeta(this DictionaryAdapterMeta dictionaryAdapterMeta, XmlMetadata xmlMeta)
		{
			dictionaryAdapterMeta.ExtendedProperties[XmlMetaKey] = xmlMeta;
		}

		private static DictionaryAdapterMeta GetDictionaryMeta(IDictionaryAdapter dictionaryAdapter, Type otherType)
		{
			var meta = dictionaryAdapter.Meta;
			if (otherType != null && otherType != meta.Type)
			{
				var descriptor = new DictionaryDescriptor();
				dictionaryAdapter.This.Descriptor.CopyBehaviors(descriptor);
				meta = dictionaryAdapter.This.Factory.GetAdapterMeta(otherType, descriptor);
			}
			return meta;
		}

		public static XPathExpression[] Split(this XPathExpression expression)
		{
			var path  = expression.Expression;
			var parts = new List<XPathExpression>();

			SplitConsumer consumer = (start, limit) =>
			{
				var subpath = path.Substring(start, limit - start);
				var subexpression = XPathExpression.Compile(subpath);
				parts.Add(subexpression);
			};

			return Split(path, consumer)
				? parts.ToArray()
				: null;
		}

		private delegate void SplitConsumer(int start, int limit);

		private static bool Split(string path, SplitConsumer consumer)
		{
			if (null == path)
				throw new ArgumentNullException("path");

			var start = 0;
			var state = 0;
			var brace = 0;

			for (var index = 0; index <= path.Length; index++)
			{
				var c = (index < path.Length)
					? path[index]
					: '/'; // terminator; enables state machine to capture last part

				switch (state)
				{
					case 0: // start
						if      (c == '/')  { state = 1; }
						else if (c == '[')  { return false; }
						else if (c == ']')  { return false; }
						else                { state = 3; start = index; }
						break;
					case 1: // after [...] '/'
						if      (c == '/')  { state = 2; }
						else if (c == '[')  { return false; }
						else if (c == ']')  { return false; }
						else                { state = 3; start = index; }
						break;
					case 2: // after [...] '/' '/'
						if      (c == '/')  { return false; }
						else if (c == '[')  { return false; }
						else if (c == ']')  { return false; }
						else                { state = 3; start = index - 2; }
						break;
					case 3: // after [[...] '/'] ['/'] * [...]
						if      (c == '/')  { goto case 8; }
						else if (c == '[')  { state = 4; brace++; }
						else if (c == ']')  { return false; }
						else                { /* NOP */ }
						break;
					case 4: // after [[...] '/'] ['/'] * [...] '['
						if      (c == '[')  { brace++; }
						else if (c == ']')  { brace--; if (brace == 0) state = 7; }
						else if (c == '\'') { state = 5; }
						else if (c == '\"') { state = 6; }
						else                { /* NOP */ }
						break;
					case 5: // after [[...] '/'] ['/'] * [...] '[' [...] '''
						if      (c == '\'') { state = 4; }
						else                { /* NOP */ }
						break;
					case 6: // after [[...] '/'] ['/'] * [...] '[' [...] '"'
						if      (c == '\"') { state = 4; }
						else                { /* NOP */ }
						break;
					case 7: // after [[...] '/'] ['/'] * [...] '[' [...] ']'
						if      (c == '/')  { goto case 8; }
						else if (c == '[')  { state = 4; brace++; }
						else                { return false; }
						break;
					case 8: // Capture
						consumer(start, index);
						state = 1;
						break;
					default:
						throw new InvalidOperationException();
				}
			}

			return state == 1;
		}
	}
#endif
}
