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
	/// Summary description for NewInstanceExpression.
	/// </summary>
	public class NewInstanceExpression : Expression
	{
		private Type _type;
		private Type[] _constructor_args;
		private Expression[] _arguments;
		private ConstructorInfo _constructor;

		public NewInstanceExpression( EasyCallable callable, params Expression[] args ) : 
			this( callable.Constructor, args )
		{
		}

		public NewInstanceExpression( ConstructorInfo constructor, params Expression[] args )
		{
			_constructor = constructor;
			_arguments = args;
		}

		public NewInstanceExpression( Type target, Type[] constructor_args, params Expression[] args )
		{
			_type = target;
			_constructor_args = constructor_args;
			_arguments = args;
		}

		public override void Emit(IEasyMember member, ILGenerator gen)
		{
			foreach(Expression exp in _arguments)
			{
				exp.Emit(member, gen);
			}

			if (_constructor == null)
			{
				_constructor = _type.GetConstructor( _constructor_args );
			}

			if (_constructor == null)
			{
				throw new ApplicationException("Could not find constructor matching specified arguments");
			}

			gen.Emit(OpCodes.Newobj, _constructor);
		}
	}
}
