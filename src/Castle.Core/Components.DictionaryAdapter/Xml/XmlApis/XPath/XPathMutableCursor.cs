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
	using System.Xml;
	using System.Xml.XPath;

	internal class XPathMutableCursor : XPathNode, IXmlCursor
	{
		private XPathBufferedNodeIterator iterator;
		private CompiledXPathStep step;
		private int depth;

		private readonly ILazy<XPathNavigator> parent;
		private readonly CompiledXPath path;
		private readonly IXmlIncludedTypeMap knownTypes;
		private readonly IXmlNamespaceSource namespaces;
		private readonly CursorFlags flags;

		public XPathMutableCursor(ILazy<XPathNavigator> parent, CompiledXPath path,
			IXmlIncludedTypeMap knownTypes, IXmlNamespaceSource namespaces, CursorFlags flags)
		{
			if (null == parent)
				throw Error.ArgumentNull("parent");
			if (null == path)
				throw Error.ArgumentNull("path");
			if (null == knownTypes)
				throw Error.ArgumentNull("knownTypes");
			if (null == namespaces)
				throw Error.ArgumentNull("namespaces");
			if (!path.IsCreatable)
				throw Error.XPathNotCreatable(path);

			this.parent     = parent;
			this.path       = path;
			this.step       = path.FirstStep;
			this.knownTypes = knownTypes;
			this.namespaces = namespaces;
			this.flags      = flags;

			if (parent.HasValue)
				CreateIterator();
		}

		private void CreateIterator()
		{
			iterator = new XPathBufferedNodeIterator
			(
				parent.Value.Select(path.FirstStep.Path)
			);
		}

		public override bool Exists
		{
			get { return HasCurrent; }
		}

		public bool HasCurrent
		{
			get { return depth == path.Depth; }
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

		public override bool IsRoot
		{
			get { return HasCurrent ? base.IsRoot : false; }
		}

		public override bool IsNil
		{
			get { return HasCurrent ? base.IsNil : true; }
		}

		bool IXmlCursor.IsNil
		{
			get { return IsNil; }
			set { Realize(); this.SetXsiNil(value); }
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

		public void SetAttribute(XmlName name, string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				if (node.MoveToAttribute(name.LocalName, name.NamespaceUri))
					node.DeleteSelf();
			}
			else if (node.MoveToAttribute(name.LocalName, name.NamespaceUri))
			{
				node.SetValue(value);
				node.MoveToParent();
			}
			else
			{
				var prefix = namespaces.GetAttributePrefix(this, name.NamespaceUri);
				node.CreateAttribute(prefix, name.LocalName, name.NamespaceUri, value);
			}
		}

		public string EnsurePrefix(string namespaceUri)
		{
			return namespaces.GetAttributePrefix(this, namespaceUri);
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
			while (depth < path.Depth)
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
			step = path.FirstStep;
			depth = 0;
		}

		private int Descend()
		{
			step = step.NextStep;
			return ++depth;
		}

		public void MoveTo(IXmlNode position)
		{
			var source = position as ILazy<XPathNavigator>;
			if (source == null || !source.HasValue)
				throw Error.CursorCannotMoveToGivenNode();

			var positionNode = source.Value;

			Reset();
			while (MoveNext())
				if (HasCurrent && node.IsSamePosition(positionNode))
					return;

			throw Error.CursorCannotMoveToGivenNode();
		}

		public override void Realize()
		{
			if (HasCurrent)
				return;
			if (!(iterator == null || iterator.IsEmpty || HasPartialOrCurrent))
				throw Error.CursorNotInRealizableState();
			Create(knownTypes.Default.ClrType);
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
			IXmlIncludedType includedType;
			if (!knownTypes.TryGet(clrType, out includedType))
				throw Error.NotXmlKnownType(clrType);

			this.SetXsiType(includedType.XsiType);
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
			node = parent.Value.Clone();
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

			var value = parent.Value.Evaluate(node.Value);
			writer.WriteValue(value);
		}

		private void SeekCurrentAfterCreate(bool moved)
		{
			RequireMoved(moved);
			if (Descend() == path.Depth)
				return;

			do
			{
				moved = step.IsAttribute
					? node.MoveToFirstAttribute()
					: node.MoveToFirstChild();
				RequireMoved(moved);
			}
			while (Descend() < path.Depth);
		}

		public void RemoveAllNext()
		{
			while (MoveNext())
				Remove();
		}

		public void Remove()
		{
			RequireRemovable();

			while (--depth > 0)
				node.MoveToParent();

			node.DeleteSelf();
			ResetCurrent();
		}

		public IXmlNode Save()
		{
			return HasCurrent ? new XPathNode(node.Clone(), type) : this;
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
