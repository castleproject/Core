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

namespace CastleTests.Components.DictionaryAdapter.Xml.Tests
{
	using System;
	using System.Xml;
	using Castle.Components.DictionaryAdapter;
	using Castle.Components.DictionaryAdapter.Xml;

	internal class DummyXmlNode : IXmlNode
	{
		public XmlNodeType NodeType
		{
			get { throw new NotImplementedException(); }
		}

		public Type ClrType
		{
			get { throw new NotImplementedException(); }
		}

		public Type BaseType
		{
			get { throw new NotImplementedException(); }
		}

		public string LocalName
		{
			get { throw new NotImplementedException(); }
		}

		public string NamespaceUri
		{
			get { throw new NotImplementedException(); }
		}

		public string XsiType
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsElement
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsAttribute
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsRoot
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsNil   { get; set; }
		public string Value { get; set; }
		
		public string Xml
		{
			get { throw new NotImplementedException(); }
		}

		public IXmlCursor SelectSelf()
		{
			throw new NotImplementedException();
		}

		public IXmlCursor SelectChildren(IXmlKnownTypeMap knownTypes, CursorFlags flags)
		{
			throw new NotImplementedException();
		}

		public IXmlCursor Select(ICompiledPath path, CursorFlags flags)
		{
			throw new NotImplementedException();
		}

		public object Evaluate(ICompiledPath path)
		{
			throw new NotImplementedException();
		}

		public void Coerce(IXmlKnownType xmlType)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		public XmlReader ReadSubtree()
		{
			throw new NotImplementedException();
		}

		public XmlWriter WriteAttributes()
		{
			throw new NotImplementedException();
		}

		public XmlWriter WriteChildren()
		{
			throw new NotImplementedException();
		}

		public bool TryRecognizeType(IXmlNode node, out Type type)
		{
			throw new NotImplementedException();
		}

		public IXmlKnownType GetXmlKnownType(Type type)
		{
			throw new NotImplementedException();
		}
	}
}
