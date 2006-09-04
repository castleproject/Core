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

namespace Castle.Rook.AST
{
	using System;


	public class AssignmentExpression : Expression
	{
		public Expression target;
		public Expression value;

		/// <summary>
		/// TODO: Add operator and change from AssignmentExpression
		/// to BinaryExpression 
		/// </summary>
		/// <param name="target"></param>
		/// <param name="value"></param>
		public AssignmentExpression(Expression target, Expression value)
		{
			this.target = target;
			this.value = value;
		}

		public Expression Target
		{
			get { return target; }
		}

		public Expression Value
		{
			get { return this.value; }
		}
	}
}
