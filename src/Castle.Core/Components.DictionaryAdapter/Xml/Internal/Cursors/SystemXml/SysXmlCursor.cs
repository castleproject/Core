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
	using System.Xml.XPath;
	using System.Xml;

	public class SysXmlCursor : SysXmlNode, IXmlCursor
	{
		private State state;
		private int index;
		private readonly IXmlKnownTypeMap knownTypes;
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

		public SysXmlCursor(IXmlNode parent, IXmlKnownTypeMap knownTypes, IXmlNamespaceSource namespaces, CursorFlags flags)
			: base(namespaces, parent)
		{
			if (null == parent)
				throw Error.ArgumentNull("parent");
			if (null == knownTypes)
				throw Error.ArgumentNull("knownTypes");

			this.knownTypes = knownTypes;
			this.flags      = flags;
			this.index      = -1;

			var source = parent.RequireRealizable<XmlNode>();
			if (source.IsReal)
				node = source.Value;
		}

		public override bool IsReal
		{
			get { return HasCurrent; }
		}

		public bool HasCurrent
		{
			get { return state > State.Initial; }
		}

		public override Type ClrType
		{
			get { return HasCurrent ? base.ClrType : knownTypes.Default.ClrType; }
		}

		public override XmlName Name
		{
			get { return HasCurrent ? base.Name : GetEffectiveName(knownTypes.Default, node); }
		}

		public override XmlName XsiType
		{
			get { return HasCurrent ? base.XsiType : knownTypes.Default.XsiType; }
		}

		public override bool IsElement
		{
			get { return HasCurrent ? base.IsElement : flags.IncludesElements(); }
		}

		public override bool IsAttribute
		{
			get { return HasCurrent ? base.IsAttribute : !flags.IncludesElements(); }
		}

		public override bool IsNil
		{
			get { return HasCurrent && base.IsNil; }
			set { Realize(); base.IsNil = value; }
		}

		public override string Value
		{
			get { return HasCurrent ? base.Value : string.Empty; }
			set { base.Value = value; } // base sets IsNil, so no need to call Realize() here
		}

		public override string Xml
		{
			get { return HasCurrent ? base.Xml : null; }
		}

		public override object Evaluate(CompiledXPath path)
		{
			return HasCurrent ? base.Evaluate(path) : null;
		}

		public bool MoveNext()
		{
			var hadCurrent = HasCurrent;
			var hasCurrent = MoveNextCore() &&
			(
				flags.AllowsMultipleItems() ||
				IsAtEnd()
			);

			if (!hasCurrent && !hadCurrent)
				state = State.Empty;

			return hasCurrent;
		}

		private bool MoveNextCore()
		{
			while (Advance())
				if (IsMatch()) return true;

			return false;
		}

		private bool IsMatch()
		{
			IXmlKnownType knownType;
			return knownTypes.TryGet(this, out knownType)
				? Try.Success(out type, knownType.ClrType)
				: Try.Failure(out type);
		}

		private bool Advance()
		{
			for (;;)
			{
				switch (state)
				{
					case State.Initial:         return AdvanceToFirstElement()  || AdvanceToFirstAttribute() || Fail(State.End);
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
			var source = position.AsRealizable<XmlNode>();
			if (source == null || !source.IsReal)
				throw Error.CursorCannotMoveToGivenNode();

			IXmlKnownType knownType;
			if (!knownTypes.TryGet(position, out knownType))
				throw Error.CursorCannotMoveToGivenNode();

			node = source.Value;
			type = knownType.ClrType;

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
			switch (state)
			{
				case State.Element:
				case State.ElementPrimed:
					MoveToParentOfElement();
					state = State.End;
					break;

				case State.Attribute:
				case State.AttributePrimed:
					MoveToParentOfAttribute();
					state = State.End;
					break;

				case State.Initial:
					state = IsAtEnd() ? State.Empty : State.End;
					break;
			}
		}

		public void Reset()
		{
			MoveToEnd();
			state = State.Initial;
			index = -1;
		}

		private void MoveToParentOfElement()
		{
			node = node.ParentNode;
		}

		private void MoveToParentOfAttribute()
		{
			node = ((XmlAttribute) node).OwnerElement;
		}

		private void MoveToRealizedParent()
		{
			var parent = Parent;
			node = parent.AsRealizable<XmlNode>().Value;
			parent.IsNil = false;
		}

		public override event EventHandler Realized;
		protected virtual void OnRealized()
		{
			if (Realized != null)
				Realized(this, EventArgs.Empty);
		}

		protected override void Realize()
		{
			if (HasCurrent)
				return;
			if (state != State.Empty)
				throw Error.CursorNotInRealizableState();
			if (!flags.SupportsMutation())
				throw Error.CursorNotMutable();
			Create(knownTypes.Default.ClrType);
			OnRealized();
		}

		public void MakeNext(Type clrType)
		{
			if (MoveNext())
				Coerce(clrType);
			else
				Create(clrType);
		}

		public void Coerce(Type clrType)
		{
			RequireCoercible();

			var knownType = knownTypes.Require(clrType);

			if (IsElement)
				CoerceElement  (knownType);
			else
				CoerceAttribute(knownType);

			this.type = knownType.ClrType;
		}

		private void CoerceElement(IXmlKnownType knownType)
		{
			var oldNode = (XmlElement) node;
			var parent  = oldNode.ParentNode;
			var name    = GetEffectiveName(knownType, parent);

			if (!XmlNameComparer.Default.Equals(this.Name, name))
			{
				var newNode = CreateElementCore(parent, name);
				parent.ReplaceChild(newNode, oldNode);

				if (knownType.XsiType != XmlName.Empty)
					this.SetXsiType(knownType.XsiType);
			}
			else this.SetXsiType(knownType.XsiType);
		}

		private void CoerceAttribute(IXmlKnownType knownType)
		{
			RequireNoXsiType(knownType);

			var oldNode = (XmlAttribute) node;
			var parent  = oldNode.OwnerElement;
			var name    = GetEffectiveName(knownType, parent);

			if (!XmlNameComparer.Default.Equals(this.Name, name))
			{
				var newNode    = CreateAttributeCore(parent, name);
				var attributes = parent.Attributes;
				attributes.RemoveNamedItem(newNode.LocalName, newNode.NamespaceURI);
				attributes.InsertBefore(newNode, oldNode);
				attributes.Remove(oldNode);
			}
		}

		public void Create(Type type)
		{
			var knownType = knownTypes.Require(type);
			var position  = RequireCreatable();

			if (flags.IncludesElements())
				CreateElement  (knownType, position);
			else
				CreateAttribute(knownType, position);

			this.type = knownType.ClrType;
		}

		private void CreateElement(IXmlKnownType knownType, XmlNode position)
		{
			var parent  = node;
			var name    = GetEffectiveName(knownType, parent);
			var element = CreateElementCore(parent, name);
			parent.InsertBefore(element, position);
			state = State.Element;

			if (knownType.XsiType != XmlName.Empty)
				this.SetXsiType(knownType.XsiType);
		}

		private void CreateAttribute(IXmlKnownType knownType, XmlNode position)
		{
			RequireNoXsiType(knownType);

			var parent    = node;
			var name      = GetEffectiveName(knownType, parent);
			var attribute = CreateAttributeCore(parent, name);
			parent.Attributes.InsertBefore(attribute, (XmlAttribute) position);
			state = State.Attribute;
		}

		private XmlElement CreateElementCore(XmlNode parent, XmlName name)
		{
			var document = parent.OwnerDocument ?? (XmlDocument) parent;
			var prefix   = Namespaces.GetElementPrefix(this, name.NamespaceUri);
			var element  = document.CreateElement(prefix, name.LocalName, name.NamespaceUri);
			node = element;
			return element;
		}

		private XmlAttribute CreateAttributeCore(XmlNode parent, XmlName name)
		{
			var document  = parent.OwnerDocument ?? (XmlDocument) parent;
			var prefix    = Namespaces.GetAttributePrefix(this, name.NamespaceUri);
			var attribute = document.CreateAttribute(prefix, name.LocalName, name.NamespaceUri);
			node = attribute;
			return attribute;
		}

		private void RequireNoXsiType(IXmlKnownType knownType)
		{
			if (knownType.XsiType != XmlName.Empty)
				throw Error.CannotSetAttribute(this);
		}

		private XmlName GetEffectiveName(IXmlKnownType knownType, XmlNode parent)
		{
			var name = knownType.Name;

			return name.NamespaceUri != null
				? name
				: name.WithNamespaceUri
				(
					parent != null
						? parent.NamespaceURI
						: string.Empty
				);
		}

		public void RemoveAllNext()
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

		public override IXmlNode Save()
		{
			return HasCurrent ? new SysXmlNode(node, type, Namespaces) : this;
		}

		private XmlNode RequireCreatable()
		{
			XmlNode position;
			switch (state)
			{
				case State.Element:   position = node; MoveToParentOfElement();   break;
				case State.Attribute: position = node; MoveToParentOfAttribute(); break;
				case State.Empty:     position = null; MoveToRealizedParent();    break;
				case State.End:       position = null; break;
				default:              throw Error.CursorNotInCreatableState();
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
				throw Error.CursorNotInRemovableState();
		}

		protected static readonly StringComparer
			DefaultComparer = StringComparer.OrdinalIgnoreCase;
	}
}
#endif
