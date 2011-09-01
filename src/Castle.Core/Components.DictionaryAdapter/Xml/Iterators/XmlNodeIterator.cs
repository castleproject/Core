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

	public class XmlNodeIterator : XmlIterator
	{
		private readonly XPathNavigator current;
		private readonly IXmlKnownTypeMap knownTypes;
		private readonly bool multiple;
		private State state;
		private Type currentType;

		protected enum State
		{
			AtEnd        = -1,
			Initial      =  0,
			InElements   =  1,
			InAttributes =  2,
			InCreated    =  3
		}

		public XmlNodeIterator(XPathNavigator parent, IXmlKnownTypeMap knownTypes, bool multiple)
		{
			if (null == parent)
				throw new ArgumentNullException("parent");
			if (null == knownTypes)
				throw new ArgumentNullException("knownTypes");

			this.current    = parent.Clone();
			this.knownTypes = knownTypes;
			this.multiple   = multiple;
		}

		public override bool HasCurrent
		{
			get { return (int) state > 0; }
		}

		public override XmlTypedNode Current
		{
			get { return HasCurrent ? GetCurrent() : OnNoCurrent(); }
		}

		private XmlTypedNode GetCurrent()
		{
			return new XmlTypedNode(current, currentType);
		}

		public override bool MoveNext()
		{
			return MoveNextCore()
				&& (multiple || RequireAtEnd());
		}

		private bool MoveNextCore()
		{
			while (Advance())
				if (IsMatch()) return true;

			return false;
		}

		private bool IsMatch()
		{
			return knownTypes.TryRecognizeType(current, out currentType);
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

		public override XPathNavigator Create(Type type)
		{
			return CreateElement(type);
		}

		protected XPathNavigator CreateElement(Type type)
		{
			RequireCreatable();

			var info = knownTypes.GetXmlKnownType(type);
			var uri  = info.NamespaceUri ?? current.NamespaceURI;
			current.AppendChildElement(null, info.LocalName, uri, null);
			current.MoveToLastChild();
			currentType = type;

			state = State.InCreated;
			return current;
		}

		protected XPathNavigator CreateAttribute(Type type)
		{
			RequireCreatable();

			var info = knownTypes.GetXmlKnownType(type);
			var uri  = info.NamespaceUri ?? current.NamespaceURI;
			current.CreateAttribute(null, info.LocalName, uri, null);
			current.MoveToAttribute(info.LocalName, uri);
			currentType = type;

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