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
	using System.Collections.Generic;
	using System.Xml.XPath;

	internal class XPathMutableIterator : XmlIterator
	{
		private readonly XPathNavigator parent;
		private readonly IList<XPathExpression> paths;
//		private readonly IXmlTypeMap predicate;
		private readonly XPathNodeIterator iterator;
		private readonly bool multiple;

		private XPathNavigator node;
		private int depth;
		private int position;

		public XPathMutableIterator(XPathNavigator parent, ICompiledPath path, bool multiple)
		{
			if (null == parent)
				throw new ArgumentNullException("parent");
			if (null == path)
				throw new ArgumentNullException("path");

			this.parent   = parent;
			this.multiple = multiple;
			this.position = -1;

			paths = path.ExpressionParts;
			RequireSupportedPath(path);

			iterator = parent.Select(paths[0]);
			iterator = new XPathBufferedNodeIterator(iterator);
		}

		public int Position
		{
			get { return position; }
		}

		public override bool HasCurrent
		{
			get { return depth == paths.Count; }
		}

		public bool HasPartialOrCurrent
		{
			get { return null != node; } // (_depth > 0 works also)
		}

		public override XmlTypedNode Current
		{
			get
			{
				return HasCurrent          ? new XmlTypedNode(node, null)  //node
					 : HasPartialOrCurrent ? default(XmlTypedNode) //null
					 : OnNoCurrent();
			}
		}

		public override bool MoveNext()
		{
			ResetCurrent();

			if (!iterator.MoveNext())
				return false;

			Consume(iterator, !multiple);
			SeekCurrent();
			position++;
			return true;
		}

		private void SeekCurrent()
		{
			while (depth < paths.Count)
			{
				var iterator = node.Select(paths[depth]);
				if (!iterator.MoveNext())
					return;

				Consume(iterator, true);
			}
		}

		private void Consume(XPathNodeIterator iterator, bool single)
		{
			var candidate = iterator.Current;
			RequireElement(candidate);
			if (single) RequireAtEnd(iterator);
			node = candidate;
			depth++;
		}

		public override XPathNavigator Create(Type type)
		{
			if (HasCurrent)
				ResetCurrent();

			var isNewItem = !HasPartialOrCurrent;
			if (isNewItem)
				node = parent.Clone();

			for (var i = depth; i < paths.Count; i++)
				RequireCreatable(GetPath(i));

			using (var writer = node.AppendChild())
				for (var i = depth; i < paths.Count; i++)
					writer.WriteStartElement(GetPath(i));

			var addedNode = SeekCurrentAfterCreate();

			if (isNewItem)
				position++;

			return addedNode;
		}

		private XPathNavigator SeekCurrentAfterCreate()
		{
			RequireMoved(node.MoveToLastChild());
			if (++depth == paths.Count)
				return node;

			var addedNode = node.Clone();

			do RequireMoved(node.MoveToFirstChild());
			while (++depth < paths.Count);

			return addedNode;
		}

		public override XPathNavigator Remove()
		{
			RequireRemovable();

			for (; depth > 1; depth--)
				node.MoveToParent();

			node.SetToNil();
			var removedNode = node;

			ResetCurrent();
			return removedNode;
		}

		private void ResetCurrent()
		{
			node = null;
			depth = 0;
		}

		private string GetPath(int index)
		{
			return paths[index].Expression;
		}

		private void RequireSupportedPath(ICompiledPath path)
		{
			if (null == paths || paths.Count == 0)
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

		private void RequireAtEnd(XPathNodeIterator iterator)
		{
			if (iterator.MoveNext())
			{
				var message = string.Format(
					"The path component '{0}' selected multiple nodes, but only one was expected.",
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
