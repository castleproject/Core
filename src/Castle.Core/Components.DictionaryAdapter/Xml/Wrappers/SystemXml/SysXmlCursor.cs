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
	using System.Xml;

	public class SysXmlCursor : SysXmlNode, IXmlCursor
	{
		private State state;
		private int index;

		private readonly ILazy<XmlNode> parent;
		private readonly IXmlTypeMap knownTypes;
		private readonly CursorFlags flags;

		protected enum State
		{
			Empty           = -4, // After last item, no items were selected
			End             = -3, // After last item, 1+ items were selected
			AttributePrimed = -2, // MoveNext will select an attribute (happens after remove)
			ElementPrimed   = -1, // MoveNext will select an element   (happens after remove)
			Initial         =  0, // Before first item
			Element         =  1, // An element   is currently selected
			Attribute       =  2  // An attribute is currently selected
		}

		public SysXmlCursor(ILazy<XmlNode> parent, IXmlTypeMap knownTypes, CursorFlags flags)
		{
			if (null == parent)
				throw new ArgumentNullException("parent");
			if (null == knownTypes)
				throw new ArgumentNullException("knownTypes");

			this.parent     = parent;
			this.knownTypes = knownTypes;
			this.flags      = flags;
			this.index      = -1;

			if (parent.HasValue)
				node = parent.Value;
		}

		public override bool Exists
		{
			get { return HasCurrent; }
		}

		public bool HasCurrent
		{
			get { return state > State.Initial; }
		}

		private IXmlType DefaultKnownType
		{
			get { return knownTypes.GetXmlType(knownTypes.BaseType); }
		}

		public override Type ClrType
		{
			get { return HasCurrent ? base.ClrType : knownTypes.BaseType; }
		}

		public override string LocalName
		{
			get { return HasCurrent ? base.LocalName : DefaultKnownType.LocalName; }
		}

		public override string NamespaceUri
		{
			get { return HasCurrent ? base.NamespaceUri : DefaultKnownType.NamespaceUri; }
		}

		public override string XsiType
		{
			get { return HasCurrent ? base.XsiType : DefaultKnownType.XsiType; }
		}

		public override bool IsElement
		{
			get { return HasCurrent ? base.IsElement : flags.IncludesElements(); }
		}

		public override bool IsAttribute
		{
			get { return HasCurrent ? base.IsAttribute : !flags.IncludesElements(); }
		}

		public override bool IsRoot
		{
			get { return false; } // Never yields root
		}

		public override bool IsNil
		{
			get { return HasCurrent ? base.IsNil : true; }
			set { Realize(); base.IsNil = value; }
		}

		public override string Value
		{
			get { return HasCurrent ? base.Value : string.Empty; }
			set { Realize(); base.Value = value; }
		}

		public override string Xml
		{
			get { return HasCurrent ? base.Xml : null; }
		}

		public bool MoveNext()
		{
			return MoveNextCore()
				&& (flags.AllowsMultipleItems() || IsAtEnd());
		}

		private bool MoveNextCore()
		{
			while (Advance())
				if (IsMatch()) return true;

			return false;
		}

		private bool IsMatch()
		{
			return knownTypes.TryGetClrType(this, out type);
		}

		private bool Advance()
		{
			for (;;)
			{
				switch (state)
				{
					case State.Initial:         return AdvanceToFirstElement()  || AdvanceToFirstAttribute() || Fail(State.Empty);
					case State.Element:         return AdvanceToNextElement()   || AdvanceToFirstAttribute() || Fail(State.End);
					case State.Attribute:       return AdvanceToNextAttribute() || Fail(State.End);
					case State.ElementPrimed:   return Succeed(State.Element);
					case State.AttributePrimed: return Succeed(State.Attribute);
					case State.End:             return false;
					case State.Empty:           return false;
				}
			}
		}

		protected virtual bool AdvanceToFirstElement()
		{
			if (!flags.IncludesElements() || node == null)
				return false;
			if (!AdvanceElement(node.FirstChild))
				return false;
			state = State.Element;
			return true;
		}

		private bool AdvanceToNextElement()
		{
			if (AdvanceElement(node.NextSibling))
				return true;
			MoveToParentOfElement();
			return false;
		}

		protected virtual bool AdvanceToFirstAttribute()
		{
			if (!flags.IncludesAttributes() || node == null)
				return false;
			if (!AdvanceAttribute(node))
				return false;
			state = State.Attribute;
			return true;
		}

		private bool AdvanceToNextAttribute()
		{
			if (AdvanceAttribute(((XmlAttribute)node).OwnerElement))
				return true;
			MoveToParentOfAttribute();
			return false;
		}

		private bool AdvanceElement(XmlNode next)
		{
			for (;;)
			{
				if (next == null)
					return false;

				if (next.NodeType == XmlNodeType.Element)
				{
					node = next;
					return true;
				}

				next = next.NextSibling;
			}
		}

		private bool AdvanceAttribute(XmlNode parent)
		{
			var attributes = parent.Attributes;

			for (;;)
			{
				index++;
				if (index >= attributes.Count)
					return false;

				var attribute = attributes[index];
				if (!attribute.IsNamespace())
				{
					node = attribute;
					return true;
				}
			}
		}

		private bool Succeed(State state)
		{
			this.state = state;
			return true;
		}

		private bool Fail(State state)
		{
			this.state = state;
			return false;
		}

		private bool IsAtEnd()
		{
			var priorNode  = node;
			var priorType  = type;
			var priorState = state;
			var priorIndex = index;

			var hasNext = MoveNextCore();

			node  = priorNode;
			type  = priorType;
			state = priorState;
			index = priorIndex;

			return !hasNext;
		}

		public void MoveTo(IXmlNode position)
		{
			var source = position as ILazy<XmlNode>;
			if (source == null || !source.HasValue)
				throw Error.CursorCannotMoveToThatNode();

			Type sourceType;
			if (!knownTypes.TryGetClrType(position, out sourceType))
				throw Error.CursorCannotMoveToThatNode();

			node = source.Value;
			type = sourceType;

			if (IsElement)
				SetMovedToElement();
			else
				SetMovedToAttribute();
		}

		private void SetMovedToElement()
		{
			state = State.Element;
			index = -1;
		}

		private void SetMovedToAttribute()
		{
			state = State.Attribute;

			var parent = ((XmlAttribute) node).OwnerElement;
			var attributes = parent.Attributes;

			for (index = 0; index < attributes.Count; index++)
				if (attributes[index] == node)
					break;
		}

		public void MoveToEnd()
		{
			MoveToParent();
			state = State.End;
		}

		public void Reset()
		{
			MoveToParent();
			state = State.Initial;
			index = -1;
		}

		private void MoveToParent()
		{
			switch (state)
			{
				case State.Element:         // Same as next
				case State.ElementPrimed:   MoveToParentOfElement();   break;
				case State.Attribute:       // Same as next
				case State.AttributePrimed: MoveToParentOfAttribute(); break;
			}
		}

		private void MoveToParentOfElement()
		{
			node = node.ParentNode;
		}

		private void MoveToParentOfAttribute()
		{
			node = ((XmlAttribute) node).OwnerElement;
		}

		public override void Realize()
		{
			if (HasCurrent)
				return;
			if (state != State.Empty)
				throw Error.CursorNotInRealizableState();
			if (!flags.SupportsMutation())
				throw Error.IteratorNotMutable();
			Create(knownTypes.BaseType);
		}

		public void MakeNext(Type type)
		{
			if (MoveNext())
				Coerce(type);
			else
				Create(type);
		}

		public void Coerce(Type type)
		{
			RequireCoercible();

			var knownType = knownTypes.GetXmlType(type);

			Coerce(knownType);
		}

		public void Create(Type type)
		{
			var position  = RequireCreatable();
			var knownType = knownTypes.GetXmlType(type);

			if (flags.IncludesElements())
			{
				CreateElement(knownType, position);
				state = State.Element;
			}
			else
			{
				CreateAttribute(knownType, position);
				state = State.Attribute;
			}
		}

		public void RemoveToEnd()
		{
			while (MoveNext())
				Remove();
		}

		public void Remove()
		{
			RequireRemovable();

			var removedNode = node;
			var wasElement  = IsElement;
			MoveNext();

			switch (state)
			{
				case State.Attribute: state = State.AttributePrimed; break;
				case State.Element:   state = State.ElementPrimed;   break;
			}

			if (wasElement)
				RemoveElement  (removedNode);
			else
				RemoveAttribute(removedNode);
		}

		private void RemoveElement(XmlNode node)
		{
			node.ParentNode.RemoveChild(node);
		}

		private void RemoveAttribute(XmlNode node)
		{
			var attribute = (XmlAttribute) node;
			attribute.OwnerElement.Attributes.Remove(attribute);
		}

		public IXmlNode Save()
		{
			return new SysXmlNode(node, type);
		}

		private XmlNode RequireCreatable()
		{
			XmlNode position;
			switch (state)
			{
				case State.Element:   position = node; MoveToParentOfElement();   break;
				case State.Attribute: position = node; MoveToParentOfAttribute(); break;
				case State.End:       position = null; break;
				case State.Empty:     position = null; node = parent.Value; break;
				default:              throw Error.IteratorNotInCreatableState();
			}
			return position;
		}

		private void RequireCoercible()
		{
			if (state <= State.Initial)
				throw Error.CursorNotInCoercibleState();
		}

		private void RequireRemovable()
		{
			if (state <= State.Initial)
				throw Error.IteratorNotInRemovableState();
		}

		protected static readonly StringComparer
			DefaultComparer = StringComparer.OrdinalIgnoreCase;
	}
}
#endif
