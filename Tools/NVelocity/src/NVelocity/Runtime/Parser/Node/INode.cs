namespace NVelocity.Runtime.Parser.Node
{
	using System;
	using System.IO;
	using NVelocity.Context;

	/// <summary>  All AST nodes must implement this interface.  It provides basic
	/// machinery for constructing the parent and child relationships
	/// between nodes.
	/// </summary>
	public interface INode
	{
		Token FirstToken { get; }

		Token LastToken { get; }

		int Type { get; }

		int Info { get; set; }

		int Line { get; }

		int Column { get; }

		/// <summary>
		/// This method is called after the node has been made the current
		/// node. It indicates that child nodes can now be added to it.
		/// </summary>
		void Open();

		/// <summary>
		/// This method is called after all the child nodes have been added.
		/// </summary>
		void Close();

		INode Parent { set; get; }

		/// <summary>
		/// This method tells the node to add its argument to the node's
		/// list of children.
		/// </summary>
		void AddChild(INode n, int i);

		/// <summary>
		/// This method returns a child node.  The children are numbered
		/// from zero, left to right.
		/// </summary>
		INode GetChild(int i);

		int ChildrenCount { get; }

		/// <summary>
		/// Accept the visitor.
		/// </summary>
		Object Accept(IParserVisitor visitor, Object data);

		Object ChildrenAccept(IParserVisitor visitor, Object data);

		Object Init(IInternalContextAdapter context, Object data);
		bool Evaluate(IInternalContextAdapter context);
		Object Value(IInternalContextAdapter context);
		bool Render(IInternalContextAdapter context, TextWriter writer);
		Object Execute(Object o, IInternalContextAdapter context);
		string Literal { get; }

		bool IsInvalid { get; set; }
	}
}
