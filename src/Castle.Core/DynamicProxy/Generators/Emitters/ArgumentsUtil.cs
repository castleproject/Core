// Copyright 2004-2026 Castle Project - http://www.castleproject.org/
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

	internal static class ArgumentsUtil
	{
		extension(ParameterInfo[] parameters)
		{
			public ArgumentReference[] ConvertToArgumentReferences()
			{
				if (parameters.Length == 0) return [];

				var arguments = new ArgumentReference[parameters.Length];

				for (var i = 0; i < parameters.Length; ++i)
				{
					arguments[i] = new ArgumentReference(parameters[i].ParameterType, i + 1);
				}

				return arguments;
			}

			public Type[] GetTypes()
			{
				if (parameters.Length == 0) return [];

				var types = new Type[parameters.Length];
				for (var i = 0; i < parameters.Length; i++)
				{
					types[i] = parameters[i].ParameterType;
				}
				return types;
			}
		}

		extension(Type[] parameterTypes)
		{
			public ArgumentReference[] ConvertToArgumentReferences()
			{
				if (parameterTypes.Length == 0) return [];

				var arguments = new ArgumentReference[parameterTypes.Length];

				for (var i = 0; i < parameterTypes.Length; ++i)
				{
					arguments[i] = new ArgumentReference(parameterTypes[i], i + 1);
				}

				return arguments;
			}
		}

		public static Type[] InitializeAndConvert(ArgumentReference[] arguments)
		{
			if (arguments.Length == 0) return [];

			var types = new Type[arguments.Length];

			for (var i = 0; i < arguments.Length; ++i)
			{
				arguments[i].Position = i + 1;
				types[i] = arguments[i].Type;
			}

			return types;
		}
	}
}