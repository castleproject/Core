using System;
using NVelocity.Runtime.Parser.Node;

namespace NVelocity.Runtime.Exception {

    public class NodeException:System.Exception {

	public NodeException(System.String exceptionMessage, INode node):base(exceptionMessage + ": " + node.literal() + " [line " + node.Line + ",column " + node.Column + "]") {}}
}
