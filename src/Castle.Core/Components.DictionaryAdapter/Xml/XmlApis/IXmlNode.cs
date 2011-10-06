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
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.f
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;
	using System.Xml;

	public interface IXmlNode : IXmlKnownType
	{
		bool   IsElement   { get; }
		bool   IsAttribute { get; }
		bool   IsRoot      { get; }
		bool   IsNil       { get; }
		string Value       { get; set; } // Equivalent to InnerText
		string Xml         { get; }      // Equivalent to OuterXml

		string GetAttribute(XmlName name);

		string LookupPrefix(string namespaceUri);
		string LookupNamespaceUri(string prefix);
		void DefineNamespace(string prefix, string namespaceUri, bool root);

		bool PositionEquals(IXmlNode node);

		IXmlCursor SelectSelf(Type clrType);
		IXmlCursor SelectChildren(IXmlKnownTypeMap knownTypes, IXmlNamespaceSource namespaces, CursorFlags flags);
#if !SL3
		IXmlCursor Select  (CompiledXPath path, IXmlIncludedTypeMap includedTypes, IXmlNamespaceSource namespaces, CursorFlags flags);
		object     Evaluate(CompiledXPath path);
#endif

		void Clear();
		XmlReader ReadSubtree();
		XmlWriter WriteAttributes();
		XmlWriter WriteChildren();
	}
}
