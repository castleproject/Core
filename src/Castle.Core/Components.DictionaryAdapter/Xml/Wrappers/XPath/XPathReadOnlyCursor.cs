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

#if !SL3
namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;
	using System.Xml.XPath;

	public class XPathReadOnlyCursor : XPathNode, IXmlCursor
	{
		private XPathNodeIterator iterator;

		private readonly XPathNavigator parent;
		private readonly XPathExpression path;
		private readonly IXmlTypeMap knownTypes;
		private readonly CursorFlags flags;

		public XPathReadOnlyCursor(ILazy<XPathNavigator> parent, ICompiledPath path, IXmlTypeMap knownTypes, CursorFlags flags)
		{
			if (parent == null)
				throw new ArgumentNullException("parent");
			if (path == null)
				throw new ArgumentNullException("path");
			if (knownTypes == null)
				throw new ArgumentNullException("knownTypes");

			this.parent     = parent.Value;
			this.path       = path.Expression;
			this.knownTypes = knownTypes;
			this.flags      = flags;

			Reset();
		}

		public void Reset()
		{
			iterator = parent.Select(path);
		}

		public bool MoveNext()
		{
			var hasNext
				=  iterator != null
				&& iterator.MoveNext()
				&& (flags.AllowsMultipleItems() || !iterator.MoveNext());

			return hasNext ? SetAtNext() : SetAtEnd();
		}

		private bool SetAtEnd()
		{
			node = null;
			type = null;
			return false;
		}

		private bool SetAtNext()
		{
			node = iterator.Current;
			if (!knownTypes.TryGetClrType(this, out type))
				type = knownTypes.BaseType;
			return true;
		}

		public void MoveTo(IXmlNode position)
		{
			var source = position as ILazy<XPathNavigator>;
			if (source == null || !source.HasValue)
				throw Error.CursorCannotMoveToThatNode();

			var positionNode = source.Value;

			Reset();
			while (iterator.MoveNext())
				if (iterator.Current.IsSamePosition(positionNode))
					{ SetAtNext(); return; }

			throw Error.CursorCannotMoveToThatNode();
		}

		public void MoveToEnd()
		{
			while (iterator.MoveNext()) ;
			SetAtEnd();
		}

		public void MakeNext(Type type)
		{
			throw Error.IteratorNotMutable();
		}

		public void Create(Type type)
		{
			throw Error.IteratorNotMutable();
		}

		public void Coerce(Type type)
		{
			throw Error.IteratorNotMutable();
		}

		public void Remove()
		{
			throw Error.IteratorNotMutable();
		}

		public void RemoveToEnd()
		{
			throw Error.IteratorNotMutable();
		}

		public IXmlNode Save()
		{
			return new XPathNode(node.Clone(), type);
		}
	}
}
#endif
