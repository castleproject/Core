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

	public enum ParameterType
	{
		Ordinary,
		Params,
		List, 
		Block
	}

	public class ParameterVarIdentifier : AbstractVarIdentifier
	{
		private readonly ParameterType paramType;

		private IExpression initExpression;

		public ParameterVarIdentifier(String name, ParameterType paramType) : base(name)
		{
			this.paramType = paramType;
		}

		public IExpression InitExpression
		{
			get { return initExpression; }
			set { initExpression = value; }
		}

		public override bool Accept(IASTVisitor visitor)
		{
			visitor.VisitParameterVarIdentifier(this);

			return true;
		}

		public static ParameterVarIdentifier FromIdentifier(ParameterType type, Identifier identifier)
		{
			ParameterVarIdentifier param = new ParameterVarIdentifier(identifier.Name, type);
			param.TypeReference = identifier.TypeReference;
			return param;
		}
	}
}
