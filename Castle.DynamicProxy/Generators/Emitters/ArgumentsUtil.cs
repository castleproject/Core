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

namespace Castle.DynamicProxy.Generators.Emitters
{
	using System;
	using System.Reflection;
	using System.Reflection.Emit;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

	public abstract class ArgumentsUtil
	{
		public static void EmitLoadOwnerAndReference(Reference reference, ILGenerator il)
		{
			if (reference == null) return;

			EmitLoadOwnerAndReference(reference.OwnerReference, il);

			reference.LoadReference(il);
		}

		public static void InitializeArgumentsByPosition(ArgumentReference[] args, bool isStatic)
		{
			int offset = isStatic ? 0 : 1;
			for (int i = 0; i < args.Length; ++i)
			{
				args[i].Position = i + offset;
			}
		}

		public static Type[] InitializeAndConvert(ArgumentReference[] args)
		{
			Type[] types = new Type[args.Length];

			for (int i = 0; i < args.Length; ++i)
			{
				args[i].Position = i + 1;
				types[i] = args[i].Type;
			}

			return types;
		}

		public static ArgumentReference[] ConvertToArgumentReference(Type[] args)
		{
			var arguments = new ArgumentReference[args.Length];

			for (int i = 0; i < args.Length; ++i)
			{
				arguments[i] = new ArgumentReference(args[i]);
			}

			return arguments;
		}

		public static bool IsAnyByRef(ParameterInfo[] parameters)
		{
			for (int i = 0; i < parameters.Length; i++)
			{
				if (parameters[i].ParameterType.IsByRef) return true;
			}
			return false;
		}

		public static ArgumentReference[] ConvertToArgumentReference(ParameterInfo[] args)
		{
			var arguments = new ArgumentReference[args.Length];

			for (int i = 0; i < args.Length; ++i)
			{
				arguments[i] = new ArgumentReference(args[i].ParameterType);
			}

			return arguments;
		}

		public static ReferenceExpression[] ConvertToArgumentReferenceExpression(ParameterInfo[] args)
		{
			var arguments = new ReferenceExpression[args.Length];

			for (int i = 0; i < args.Length; ++i)
			{
				arguments[i] = new ReferenceExpression(new ArgumentReference(args[i].ParameterType, i + 1));
			}

			return arguments;
		}
		public static Expression[] ConvertArgumentReferenceToExpression(ArgumentReference[] args)
		{
			Expression[] expressions = new Expression[args.Length];

			for (int i = 0; i < args.Length; ++i)
			{
				expressions[i] = args[i].ToExpression();
			}

			return expressions;
		}
	}
}