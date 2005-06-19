<<<<<<< .mine
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
	using System.Reflection;
	using System.Reflection.Emit;

	/// <summary>
	/// Summary description for MethodTokenExpression.
	/// </summary>
	public class MethodTokenExpression : Expression
	{
		private MethodInfo _method;

		public MethodTokenExpression( MethodInfo method )
		{
			_method = method;
		}

		public override void Emit(IEasyMember member, ILGenerator gen)
		{
			gen.Emit(OpCodes.Ldtoken, _method);
			
			MethodInfo minfo = typeof(MethodBase).GetMethod(
				"GetMethodFromHandle", BindingFlags.Static|BindingFlags.Public, null, 
				new Type[] { typeof(RuntimeMethodHandle) }, null);

			gen.Emit(OpCodes.Call, minfo);

			gen.Emit(OpCodes.Castclass, typeof(MethodInfo));
		}
	}
}
=======
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
	using System.Reflection;
	using System.Reflection.Emit;

	/// <summary>
	/// Summary description for MethodTokenExpression.
	/// </summary>
	public class MethodTokenExpression : Expression
	{
		private MethodInfo _method;

		public MethodTokenExpression( MethodInfo method )
		{
			_method = method;
		}

		public override void Emit(IEasyMember member, ILGenerator gen)
		{
			gen.Emit(OpCodes.Ldtoken, _method);
			gen.Emit(OpCodes.Call, typeof(MethodBase).GetMethod(
				"GetMethodFromHandle", new Type[] { typeof(RuntimeMethodHandle) }));
			gen.Emit(OpCodes.Castclass, typeof(MethodInfo));
		}
	}
}
>>>>>>> .r637
