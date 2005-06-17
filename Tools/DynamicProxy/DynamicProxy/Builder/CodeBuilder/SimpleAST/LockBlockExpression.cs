using System.Collections;
using System.Threading;
// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST
{
	using System;
	using System.Reflection.Emit;

	/// <summary>
	/// Summary description for LockBlockExpression.
	/// </summary>
	public class LockBlockExpression : Expression
	{
		private Reference _syncLockSource;
		private ArrayList _stmts;

		public LockBlockExpression( Reference syncLockSource )
		{
			_syncLockSource = syncLockSource;
			_stmts = new ArrayList();
		}

		public void AddStatement(Statement stmt)
		{
			_stmts.Add(stmt);
		}

		public override void Emit(IEasyMember member, ILGenerator gen)
		{
			_syncLockSource.LoadReference(gen);
			gen.Emit(OpCodes.Call, typeof(Monitor).GetMethod("Enter"));

			Label tryBlock = gen.BeginExceptionBlock();

			foreach(Statement stmt in _stmts)
			{
				stmt.Emit(member, gen);
			}


			gen.BeginFinallyBlock();
			_syncLockSource.LoadReference(gen);
			gen.Emit(OpCodes.Call, typeof(Monitor).GetMethod("Exit"));
			gen.EndExceptionBlock();

		}
	}
}
