using System;
using System.Collections.Generic;
using System.Xml.XPath;

namespace Castle.Components.DictionaryAdapter
{
	internal class XPathMutableIterator : SimpleIterator<XPathNavigator>
	{
		private readonly XPathNavigator _parent;
		private readonly IList<XPathExpression> _paths;
		private readonly XPathNodeIterator _iterator;
		private readonly bool _singleItem;

		private XPathNavigator _node;
		private int _depth;
		private int _position;

		public XPathMutableIterator(XPathNavigator parent, ICompiledPath path, bool singleItem = false)
		{
			if (null == parent)
				throw new ArgumentNullException("parent");
			if (null == path)
				throw new ArgumentNullException("path");

			_parent = parent;
			_singleItem = singleItem;
			_position = -1;

			_paths = path.ExpressionParts;
			RequireSupportedPath(path);

			_iterator = parent.Select(_paths[0]);
			_iterator = new XPathBufferedNodeIterator(_iterator);
		}

		public int Position
		{
			get { return _position; }
		}

		public bool HasCurrent
		{
			get { return _depth == _paths.Count; }
		}

		public bool HasPartialOrCurrent
		{
			get { return null != _node; } // (_depth > 0 works also)
		}

		public override XPathNavigator Current
		{
			get
			{
				return HasCurrent          ? _node
					 : HasPartialOrCurrent ? null
					 : OnNoCurrent();
			}
		}

		public override bool MoveNext()
		{
			ResetCurrent();

			if (!_iterator.MoveNext())
				return false;

			Consume(_iterator, _singleItem);
			SeekCurrent();
			_position++;
			return true;
		}

		private void SeekCurrent()
		{
			while (_depth < _paths.Count)
			{
				var iterator = _node.Select(_paths[_depth]);
				if (!iterator.MoveNext())
					return;

				Consume(iterator, singleItem: true);
			}
		}

		private void Consume(XPathNodeIterator iterator, bool singleItem)
		{
			var candidate = iterator.Current;
			RequireElement(candidate);
			if (singleItem) RequireAtEnd(iterator);
			_node = candidate;
			_depth++;
		}

		public XPathNavigator Create()
		{
			if (HasCurrent)
				ResetCurrent();

			var isNewItem = !HasPartialOrCurrent;
			if (isNewItem)
				_node = _parent.Clone();

			for (var i = _depth; i < _paths.Count; i++)
				RequireCreatable(GetPath(i));

			using (var writer = _node.AppendChild())
				for (var i = _depth; i < _paths.Count; i++)
					writer.WriteStartElement(GetPath(i));

			var addedNode = SeekCurrentAfterCreate();

			if (isNewItem)
				_position++;

			return addedNode;
		}

		private XPathNavigator SeekCurrentAfterCreate()
		{
			RequireMoved(_node.MoveToLastChild());
			if (++_depth == _paths.Count)
				return _node;

			var addedNode = _node.Clone();

			do RequireMoved(_node.MoveToFirstChild());
			while (++_depth < _paths.Count);

			return addedNode;
		}

		public XPathNavigator Remove()
		{
			RequireRemovable();

			for (; _depth > 1; _depth--)
				_node.MoveToParent();

			_node.SetToNil();
			var removedNode = _node;

			ResetCurrent();
			return removedNode;
		}

		private void ResetCurrent()
		{
			_node = null;
			_depth = 0;
		}

		private string GetPath(int index)
		{
			return _paths[index].Expression;
		}

		private void RequireSupportedPath(ICompiledPath path)
		{
			if (null == _paths || _paths.Count == 0)
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
					GetPath(_depth));
				throw new XPathException(message);
			}
		}

		private void RequireAtEnd(XPathNodeIterator iterator)
		{
			if (iterator.MoveNext())
			{
				var message = string.Format(
					"The path component '{0}' selected multiple nodes, but only one was expected.",
					GetPath(_depth));
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
					GetPath(_depth));
				throw new XPathException(message);
			}
		}
	}
}
