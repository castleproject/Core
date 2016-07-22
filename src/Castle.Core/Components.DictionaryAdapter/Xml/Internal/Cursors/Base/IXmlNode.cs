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

#if FEATURE_DICTIONARYADAPTER_XML
namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;
	using System.Xml;

	public interface IXmlNode : IXmlKnownType, IRealizableSource, IVirtual
	{
		bool   IsElement   { get; }
		bool   IsAttribute { get; }
		bool   IsNil       { get; set; }
		string Value       { get; set; } // Equivalent to InnerText
		string Xml         { get; }      // Equivalent to OuterXml

		IXmlNode            Parent          { get; }
		IXmlNamespaceSource Namespaces      { get; }

		string GetAttribute(XmlName name);
		void   SetAttribute(XmlName name, string value);

		string LookupPrefix      (string namespaceUri);
		string LookupNamespaceUri(string prefix);
		void   DefineNamespace   (string prefix, string namespaceUri, bool root);

		object UnderlyingObject { get; }
		bool UnderlyingPositionEquals(IXmlNode node);

		IXmlNode     Save();
		IXmlCursor   SelectSelf(Type clrType);
		IXmlCursor   SelectChildren(IXmlKnownTypeMap knownTypes, IXmlNamespaceSource namespaces, CursorFlags flags);
		IXmlIterator SelectSubtree();

		CompiledXPath Path    { get; }
		IXmlCursor    Select  (CompiledXPath path, IXmlIncludedTypeMap includedTypes, IXmlNamespaceSource namespaces, CursorFlags flags);
		object        Evaluate(CompiledXPath path);

		void      Clear();
		XmlReader ReadSubtree();
		XmlWriter WriteAttributes();
		XmlWriter WriteChildren();
	}
}
#endif
