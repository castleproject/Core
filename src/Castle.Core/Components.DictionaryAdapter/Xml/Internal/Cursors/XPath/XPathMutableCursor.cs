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
	using System.Xml.XPath;

	internal class XPathMutableCursor : XPathNode, IXmlCursor
	{
		private XPathBufferedNodeIterator iterator;
		private CompiledXPathStep step;
		private int depth;

		private readonly IXmlIncludedTypeMap knownTypes;
		private readonly CursorFlags flags;

		public XPathMutableCursor(IXmlNode parent, CompiledXPath path,
			IXmlIncludedTypeMap knownTypes, IXmlNamespaceSource namespaces, CursorFlags flags)
			: base(path, namespaces, parent)
		{
			if (null == parent)
				throw Error.ArgumentNull("parent");
			if (null == path)
				throw Error.ArgumentNull("path");
			if (null == knownTypes)
				throw Error.ArgumentNull("knownTypes");
			if (!path.IsCreatable)
				throw Error.XPathNotCreatable(path);

			this.step       = path.FirstStep;
			this.knownTypes = knownTypes;
			this.flags      = flags;

			var source = parent.RequireRealizable<XPathNavigator>();
			if (source.IsReal)
				iterator = new XPathBufferedNodeIterator(
					source.Value.Select(path.FirstStep.Path));
		}

		public override bool IsReal
		{
			get { return HasCurrent; }
		}

		public bool HasCurrent
		{
			get { return depth == xpath.Depth; }
		}

		public bool HasPartialOrCurrent
		{
			get { return null != node; } // (_depth > 0 works also)
		}

		public override Type ClrType
		{
			get { return HasCurrent ? base.ClrType : knownTypes.Default.ClrType; }
		}

		public override XmlName Name
		{
			get { return HasCurrent ? base.Name : XmlName.Empty; }
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
			ResetCurrent();

			for (;;)
			{
				var hasNext
					=  iterator != null
					&& iterator.MoveNext()
					&& Consume(iterator, flags.AllowsMultipleItems());

				if (!hasNext)
					return SetAtEnd();
				if (SeekCurrent())
					return true;
			}
		}

		private bool SeekCurrent()
		{
			while (depth < xpath.Depth)
			{
				var iterator = node.Select(step.Path);
				if (!iterator.MoveNext())
					return true; // Sought as far as possible
				if (!Consume(iterator, false))
					return false; // Problem: found multiple nodes
			}

			IXmlIncludedType includedType;
			if (!knownTypes.TryGet(XsiType, out includedType))
				return false; // Problem: unrecognized xsi:type

			type = includedType.ClrType;
			return true; // Sought all the way
		}

		private bool Consume(XPathNodeIterator iterator, bool multiple)
		{
			var candidate = iterator.Current;
			if (!multiple && iterator.MoveNext())
				return false;

			node = candidate;
			Descend();
			return true;
		}

		private bool SetAtEnd()
		{
			ResetCurrent();
			return false;
		}

		public void Reset()
		{
			ResetCurrent();
			iterator.Reset();
		}

		public void MoveToEnd()
		{
			ResetCurrent();
			iterator.MoveToEnd();
		}

		private void ResetCurrent()
		{
			node = null;
			type = null;
			ResetDepth();
		}

		private void ResetDepth()
		{
			step = xpath.FirstStep;
			depth = 0;
		}

		private int Descend()
		{
			step = step.NextStep;
			return ++depth;
		}

		public void MoveTo(IXmlNode position)
		{
			var source = position.AsRealizable<XPathNavigator>();
			if (source == null || !source.IsReal)
				throw Error.CursorCannotMoveToGivenNode();

			var positionNode = source.Value;

			Reset();
			while (MoveNext())
				if (HasCurrent && node.IsSamePosition(positionNode))
					return;

			throw Error.CursorCannotMoveToGivenNode();
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
			if (!(iterator == null || iterator.IsEmpty || HasPartialOrCurrent))
				throw Error.CursorNotInRealizableState();
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
			var includedType = knownTypes.Require(clrType);
			this.SetXsiType(includedType.XsiType);
			this.type = clrType;
		}

		public void Create(Type type)
		{
			if (HasCurrent)
				Insert();
			else if (HasPartialOrCurrent)
				Complete();
			else
				Append();

			Coerce(type);
		}

		private void Insert()
		{
			while (--depth > 0)
				node.MoveToParent();
			ResetDepth();

			using (var writer = node.InsertBefore())
				WriteNode(step, writer);

			var moved = node.MoveToPrevious();
			SeekCurrentAfterCreate(moved);
		}

		private void Append()
		{
			node = Parent.AsRealizable<XPathNavigator>().Value.Clone();
			Parent.IsNil = false;
			Complete();
		}

		private void Complete()
		{
			using (var writer = CreateWriterForAppend())
				WriteNode(step, writer);

			var moved = step.IsAttribute
				? node.MoveToLastAttribute()
				: node.MoveToLastChild();
			SeekCurrentAfterCreate(moved);
		}

		private XmlWriter CreateWriterForAppend()
		{
			return step.IsAttribute
				? node.CreateAttributes()
				: node.AppendChild();
		}

		private void WriteNode(CompiledXPathNode node, XmlWriter writer)
		{
			if (node.IsAttribute)
				WriteAttribute(node, writer);
			else if (node.IsSimple)
				WriteSimpleElement(node, writer);
			else
				WriteComplexElement(node, writer);
		}

		private void WriteAttribute(CompiledXPathNode node, XmlWriter writer)
		{
			writer.WriteStartAttribute(node.Prefix, node.LocalName, null);
			WriteValue(node, writer);
			writer.WriteEndAttribute();
		}

		private void WriteSimpleElement(CompiledXPathNode node, XmlWriter writer)
		{
			writer.WriteStartElement(node.Prefix, node.LocalName, null);
			WriteValue(node, writer);
			writer.WriteEndElement();
		}

		private void WriteComplexElement(CompiledXPathNode node, XmlWriter writer)
		{
			writer.WriteStartElement(node.Prefix, node.LocalName, null);
			WriteSubnodes(node, writer, true);
			WriteSubnodes(node, writer, false);
			writer.WriteEndElement();
		}

		private void WriteSubnodes(CompiledXPathNode parent, XmlWriter writer, bool attributes)
		{
			var next = parent.NextNode;
			if (next != null && next.IsAttribute == attributes)
				WriteNode(next, writer);

			foreach (var node in parent.Dependencies)
				if (node.IsAttribute == attributes)
					WriteNode(node, writer);
		}

		private void WriteValue(CompiledXPathNode node, XmlWriter writer)
		{
			if (node.Value == null)
				return;

			var value = Parent.AsRealizable<XPathNavigator>().Value.Evaluate(node.Value);
			writer.WriteValue(value);
		}

		private void SeekCurrentAfterCreate(bool moved)
		{
			RequireMoved(moved);
			if (Descend() == xpath.Depth)
				return;

			do
			{
				moved = step.IsAttribute
					? node.MoveToFirstAttribute()
					: node.MoveToFirstChild();
				RequireMoved(moved);
			}
			while (Descend() < xpath.Depth);
		}

		public void RemoveAllNext()
		{
			while (MoveNext())
				Remove();
		}

		public void Remove()
		{
			RequireRemovable();

			var name = XmlName.Empty;

			if (!HasCurrent)
			{
				var namespaceUri = LookupNamespaceUri(step.Prefix) ?? node.NamespaceURI;
				name = new XmlName(step.LocalName, namespaceUri);
			}

			do
			{
				if (node.MoveToChild(name.LocalName, name.NamespaceUri))
					break;

				name = new XmlName(node.LocalName, node.NamespaceURI);
				node.DeleteSelf();
				depth--;
			}
			while (depth > 0);

			ResetCurrent();
		}

		public override IXmlNode Save()
		{
			return HasCurrent ? new XPathNode(node.Clone(), type, Namespaces) : this;
		}	

		private void RequireRemovable()
		{
			if (!HasPartialOrCurrent)
				throw Error.CursorNotInRemovableState();
		}

		private void RequireMoved(bool result)
		{
			if (!result)
				throw Error.XPathNavigationFailed(step.Path);
		}
	}
}
#endif
