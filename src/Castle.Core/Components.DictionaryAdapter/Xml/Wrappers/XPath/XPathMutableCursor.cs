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
using System.Xml;

	internal class XPathMutableCursor : XPathNode, IXmlCursor
	{
		private XPathBufferedNodeIterator iterator;
		private int depth;

		private readonly ILazy<XPathNavigator> parent;
		private readonly ICompiledPath path;
		private readonly IXmlTypeMap knownTypes;
		private readonly CursorFlags flags;

		public XPathMutableCursor(ILazy<XPathNavigator> parent, ICompiledPath path, IXmlTypeMap knownTypes, CursorFlags flags)
		{
			if (null == parent)
				throw new ArgumentNullException("parent");
			if (null == path)
				throw new ArgumentNullException("path");
			if (knownTypes == null)
				throw new ArgumentNullException("knownTypes");

			RequireSupportedPath(path);

			this.parent     = parent;
			this.path       = path;
			this.knownTypes = knownTypes;
			this.flags      = flags;

			if (parent.HasValue)
				CreateIterator();
		}

		private void CreateIterator()
		{
			iterator = new XPathBufferedNodeIterator
			(
				parent.Value.Select(path.ExpressionParts[0])
			);
		}

		public override bool Exists
		{
			get { return HasCurrent; }
		}

		public bool HasCurrent
		{
			get { return depth == path.ExpressionParts.Count; }
		}

		public bool HasPartialOrCurrent
		{
			get { return null != node; } // (_depth > 0 works also)
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
			get { return HasCurrent ? base.IsRoot : false; }
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

		private void ResetCurrent()
		{
			node = null;
			type = null;
			depth = 0;
		}

		private bool SeekCurrent()
		{
			while (depth < path.ExpressionParts.Count)
			{
				var iterator = node.Select(path.ExpressionParts[depth]);
				if (!iterator.MoveNext())
					return true; // Sought as far as possible
				if (!Consume(iterator, false))
					return false; // Problem: found multiple nodes
			}

			if (!knownTypes.TryGetClrType(this, out type))
				type = knownTypes.BaseType;
			return true; // Sought all the way
		}

		private bool Consume(XPathNodeIterator iterator, bool multiple)
		{
			var candidate = iterator.Current;
			if (!multiple && iterator.MoveNext())
				return false;

			node = candidate;
			depth++;
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

		public void MoveTo(IXmlNode position)
		{
			var source = position as ILazy<XPathNavigator>;
			if (source == null || !source.HasValue)
				throw Error.CursorCannotMoveToThatNode();

			var positionNode = source.Value;

			Reset();
			while (MoveNext())
				if (HasCurrent && node.IsSamePosition(positionNode))
					return;

			throw Error.CursorCannotMoveToThatNode();
		}

		public override void Realize()
		{
			if (HasCurrent)
				return;
			if (iterator != null && !iterator.IsEmpty)
				throw Error.CursorNotInRealizableState();
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
			var knownType = knownTypes.GetXmlType(type);
			Coerce(knownType);
		}

		public void Create(Type type)
		{
			for (var i = depth; i < path.ExpressionParts.Count; i++)
				RequireCreatable(GetPath(i));
			
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
			while (depth-- > 1)
				node.MoveToParent();

			using (var writer = node.InsertBefore())
				WriteParts(writer);

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
			using (var writer = node.AppendChild())
				WriteParts(writer);

			var moved = node.MoveToLastChild();
			SeekCurrentAfterCreate(moved);
		}

		private void WriteParts(XmlWriter writer)
		{
			for (var i = depth; i < path.ExpressionParts.Count; i++)
				writer.WriteStartElement(GetPath(i));
		}

		private void SeekCurrentAfterCreate(bool moved)
		{
			RequireMoved(moved);
			if (++depth == path.ExpressionParts.Count)
				return;

			do RequireMoved(node.MoveToFirstChild());
			while (++depth < path.ExpressionParts.Count);
		}

		public void RemoveToEnd()
		{
			while (MoveNext())
				Remove();
		}

		public void Remove()
		{
			RequireRemovable();

			while (depth-- > 1)
				node.MoveToParent();

			node.DeleteSelf();

			ResetCurrent();
		}

		public IXmlNode Save()
		{
			return HasCurrent ? new XPathNode(node.Clone(), type) : this;
		}

		private string GetPath(int index)
		{
			return path.ExpressionParts[index].Expression;
		}

		private void RequireSupportedPath(ICompiledPath path)
		{
			if (null == path.ExpressionParts || path.ExpressionParts.Count == 0)
			{
				var message = string.Format(
					"The path '{0}' is not a supported XPath path expression.",
					path.Path);
				throw new FormatException(message);
			}
		}

		private void RequireElement(XPathNavigator candidate)
		{
			if (candidate.NodeType != XPathNodeType.Element)
			{
				var message = string.Format(
					"The path component '{0}' selected a non-element node.",
					GetPath(depth));
				throw new XPathException(message);
			}
		}

		private void RequireCreatable(string path)
		{
			if (!XPath.IsNCName(path))
			{
				ResetCurrent();
				var message = string.Format(
					"Cannot create an element for path component '{0}'. The path component is not a valid XML element name (NCName)",
					path);
				throw new XPathException(message);
			}
		}

		private void RequireRemovable()
		{
			if (!HasPartialOrCurrent)
			{
				var message = string.Format(
					"Cannot remove current element at path component '{0}'. No current element is selected.",
					GetPath(0));
				throw new XPathException(message);
			}
		}

		private void RequireMoved(bool result)
		{
			if (!result)
			{
				var message = string.Format(
					"Failed navigation to {0} element after creation.",
					GetPath(depth));
				throw new XPathException(message);
			}
		}
	}
}
#endif
