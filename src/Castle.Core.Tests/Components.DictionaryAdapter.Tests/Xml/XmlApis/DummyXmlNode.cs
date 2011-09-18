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
	using Castle.Components.DictionaryAdapter.Xml;

	internal class DummyXmlNode : IXmlNode
	{
		private Type clrType;

		public DummyXmlNode()
			: this(typeof(object)) { }

		public DummyXmlNode(Type clrType)
		{
			this.clrType = clrType;
		}

		public Type ClrType
		{
			get { return clrType; }
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

		public bool IsNil   { get; set; }
		public string Value { get; set; }

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

		public string Xml
		{
			get { throw new NotImplementedException(); }
		}

		public bool PositionEquals(IXmlNode node)
		{
			throw new NotImplementedException();
		}

		public IXmlCursor SelectSelf(Type clrType)
		{
			throw new NotImplementedException();
		}

		public IXmlCursor SelectChildren(IXmlKnownTypeMap knownTypes, CursorFlags flags)
		{
			throw new NotImplementedException();
		}

		public IXmlCursor Select(ICompiledPath path, IXmlIncludedTypeMap includedTypes, CursorFlags flags)
		{
			throw new NotImplementedException();
		}

		public object Evaluate(ICompiledPath path)
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
	}
}
