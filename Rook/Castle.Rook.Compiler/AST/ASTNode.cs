// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.Rook.Compiler.AST
{
	using System;
	using Castle.Rook.Compiler.Visitors;


	public abstract class ASTNode : IASTNode
	{
		private IASTNode parent;
		private LexicalPosition position;
		private ISymbolTable symTable;

		public ASTNode()
		{
		}

		public ASTNode(ISymbolTable symTable)
		{
			this.symTable = symTable;
		}

		public IASTNode Parent
		{
			get { return parent; }
			set { parent = value; }
		}

		public LexicalPosition Position
		{
			get { return position; }
			set { position = value; }
		}

		public ISymbolTable DefiningSymbolTable
		{
			get { return symTable;  } 
			set { symTable = value; }
		}

		public virtual void ReplaceBy(IASTNode node)
		{
			if (Parent != null)
			{
				throw new ApplicationException("Couldn't find a parent node to delegate");
			}

			Parent.ReplaceChild(this, node);
		}

		public virtual void ReplaceChild(IASTNode oldNode, IASTNode newNode)
		{
			throw new NotImplementedException( this.GetType().FullName + " must implement ReplaceChild" );
		}

		public virtual void RemoveChild(IASTNode node)
		{
			throw new NotImplementedException( this.GetType().FullName + " must implement RemoveChild" );
		}

		public virtual bool Accept(IASTVisitor visitor)
		{
			throw new NotImplementedException( this.GetType().FullName + " must implement Accept" );
		}
	}
}
