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

#if !SILVERLIGHT
namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;
	using System.Xml.XPath;

	public class XmlIterator : Iterator<XPathNavigator>
	{
		private readonly string localName;
		private readonly string namespaceUri;
		private readonly bool multiple;

		private XPathNavigator current;
		private State state;

		protected enum State
		{
			AtEnd        = -1,
			Initial      =  0,
			InElements   =  1,
			InAttributes =  2,
			InCreated    =  3
		}

		public XmlIterator(XPathNavigator parent, string localName, string namespaceUri, bool multiple)
		{
			if (null == parent)
				throw new ArgumentNullException("parent");
			if (null == localName)
				throw new ArgumentNullException("localName");

			this.current      = parent.Clone();
			this.localName    = localName;
			this.namespaceUri = namespaceUri;
			this.multiple     = multiple;
		}

		public override bool HasCurrent
		{
			get { return (int) state > 0; }
		}

		public override XPathNavigator Current
		{
			get { return HasCurrent ? current : OnNoCurrent(); }
		}

		public override bool MoveNext()
		{
			return MoveNextCore()
				&& (multiple || RequireAtEnd());
		}

		private bool MoveNextCore()
		{
			while (Advance())
				if (IsMatch) return true;

			return false;
		}

		private bool Advance()
		{
			for (;;)
			{
				switch (state)
				{
					case State.Initial:      return MoveToFirstElement()  || MoveToFirstAttribute() || MoveToEnd();
					case State.InElements:   return MoveToNextElement()   || MoveToFirstAttribute() || MoveToEnd();
					case State.InAttributes: return MoveToNextAttribute() || MoveToEnd();
					case State.InCreated:    return MoveToParent()        || MoveToEnd();
					case State.AtEnd:        return false;
				}
			}
		}

		protected virtual bool MoveToFirstElement()
		{
			if (current.MoveToFirstChild())
			{
				state = State.InElements;
				return IsElement || MoveToNextElement();
			}
			return false;
		}

		private bool MoveToNextElement()
		{
			while (current.MoveToNext())
				if (IsElement) return true;

			return MoveToParent();
		}

		protected virtual bool MoveToFirstAttribute()
		{
			if (current.MoveToFirstAttribute())
			{
				state = State.InAttributes;
				return true;
			}
			return false;
		}

		private bool MoveToNextAttribute()
		{
			if (current.MoveToNextAttribute())
				return true;

			return MoveToParent();
		}

		private bool MoveToParent()
		{
			current.MoveToParent();
			return false;
		}

		private bool MoveToEnd()
		{
			state = State.AtEnd;
			return false;
		}

		private bool RequireAtEnd()
		{
			var priorState = state;
			var priorCurrent = current.Clone();

			var hasNext = MoveNextCore();

			current.MoveTo(priorCurrent);
			state = priorState;

			if (hasNext)
				throw Error.SelectedMultipleNodes();

			return true;
		}

		private bool IsElement
		{
			get { return current.NodeType == XPathNodeType.Element; }
		}

		private bool IsMatch
		{
			get
			{
				return
					DefaultComparer.Equals(current.LocalName, localName) &&
					(
						namespaceUri == null ||
						DefaultComparer.Equals(current.NamespaceURI, namespaceUri)
					);
			}
		}

		public override XPathNavigator Create()
		{
			return CreateElement();
		}

		protected XPathNavigator CreateElement()
		{
			RequireCreatable();

			var uri = namespaceUri ?? current.NamespaceURI;

			current.AppendChildElement(null, localName, uri, null);
			current.MoveToLastChild();

			state = State.InCreated;
			return current;
		}

		protected XPathNavigator CreateAttribute()
		{
			RequireCreatable();

			var uri = namespaceUri ?? current.NamespaceURI;

			current.CreateAttribute(null, localName, uri, null);
			current.MoveToAttribute(localName, uri);

			state = State.InCreated;
			return current;
		}

		private void RequireCreatable()
		{
			switch (state)
			{
				case State.InCreated: current.MoveToParent(); return;
				case State.AtEnd:     return;
				default:              throw Error.IteratorNotInCreatableState();
			}
		}

		public override XPathNavigator Remove()
		{
			RequireRemovable();

			current.DeleteSelf();

			state = State.AtEnd;
			return current;
		}

		private void RequireRemovable()
		{
			switch (state)
			{
				case State.InElements:   // Same as next
				case State.InAttributes: // Same as next
				case State.InCreated:    return;
				default:                 throw Error.IteratorNotInRemovableState();
			}
		}

		protected static readonly StringComparer
			DefaultComparer = StringComparer.OrdinalIgnoreCase;
	}
}
#endif