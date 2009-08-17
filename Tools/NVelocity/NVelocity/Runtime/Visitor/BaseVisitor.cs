// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace NVelocity.Runtime.Visitor
{
	using System;
	using System.IO;
	using Context;
	using NVelocity.Runtime.Parser.Node;

	/// <summary> This is the base class for all visitors.
	/// For each AST node, this class will provide
	/// a bare-bones method for traversal.
	/// *
	/// </summary>
	/// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
	/// </author>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
	/// </author>
	/// <version> $Id: BaseVisitor.cs,v 1.3 2003/10/27 13:54:11 corts Exp $
	///
	/// </version>
	public abstract class BaseVisitor : IParserVisitor
	{
		/// <summary>
		/// Context used during traversal
		/// </summary>
		protected internal IInternalContextAdapter context;

		/// <summary>
		/// Writer used as the output sink
		/// </summary>
		protected internal StreamWriter writer;

		public virtual Object Visit(SimpleNode node, Object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual Object Visit(ASTprocess node, Object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual Object Visit(ASTExpression node, Object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual Object Visit(ASTAssignment node, Object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual Object Visit(ASTOrNode node, Object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual Object Visit(ASTAndNode node, Object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual Object Visit(ASTEQNode node, Object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual Object Visit(ASTNENode node, Object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual Object Visit(ASTLTNode node, Object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual Object Visit(ASTGTNode node, Object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual Object Visit(ASTLENode node, Object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual Object Visit(ASTGENode node, Object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual Object Visit(ASTAddNode node, Object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual Object Visit(ASTSubtractNode node, Object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual Object Visit(ASTMulNode node, Object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual Object Visit(ASTDivNode node, Object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual Object Visit(ASTModNode node, Object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual Object Visit(ASTNotNode node, Object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual Object Visit(ASTNumberLiteral node, Object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual Object Visit(ASTStringLiteral node, Object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual Object Visit(ASTIdentifier node, Object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual Object Visit(ASTMethod node, Object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual Object Visit(ASTReference node, Object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual Object Visit(ASTTrue node, Object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual Object Visit(ASTFalse node, Object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual Object Visit(ASTBlock node, Object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual Object Visit(ASTText node, Object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual Object Visit(ASTIfStatement node, Object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual Object Visit(ASTElseStatement node, Object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual Object Visit(ASTElseIfStatement node, Object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual Object Visit(ASTComment node, Object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual Object Visit(ASTObjectArray node, Object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual Object Visit(ASTMap node, Object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual Object Visit(ASTWord node, Object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual Object Visit(ASTSetDirective node, Object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}

		public virtual Object Visit(ASTDirective node, Object data)
		{
			data = node.ChildrenAccept(this, data);
			return data;
		}
	}
}