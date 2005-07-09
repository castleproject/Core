namespace NVelocity.Runtime.Exception
{
	using System;
	using NVelocity.Runtime.Parser.Node;

	public class NodeException : Exception
	{
		public NodeException(String exceptionMessage, INode node) : base(exceptionMessage + ": " + node.literal() + " [line " + node.Line + ",column " + node.Column + "]")
		{
		}
	}
}