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

namespace Castle.DynamicProxy
{
	using System;
	using System.Reflection;

	/// <summary>
	/// Summary description for AssertUtil.
	/// </summary>
	internal abstract class AssertUtil
	{
		public static void NotNull(object argument, String argumentName)
		{
			if (argument == null)
			{
				String message = String.Format("Argument '{0}' can't be null", argumentName);
				throw new ArgumentNullException(message);
			}
		}

		public static void IsInterface(Type type, String argumentName)
		{
			NotNull(type, argumentName);

			if (!type.IsInterface)
			{
				String message = String.Format("Argument '{0}' must be an interface", argumentName);
				throw new ArgumentException(message);
			}
		}

		public static void IsInterface(Type[] types, String argumentName)
		{
			NotNull(types, argumentName);

			foreach (Type type in types)
			{
				IsInterface(type, argumentName);
			}
		}

		public static void IsClass(Type type, String argumentName)
		{
			NotNull(type, argumentName);

			if (!type.IsClass || type.IsAbstract)
			{
				bool hasAbstractMembers = false;
				
				if (type.IsAbstract)
				{ 
					MethodInfo[] abstractMethods = 
						type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					
					foreach (MethodInfo methodInfo in abstractMethods)
					{
						if (methodInfo.IsAbstract)
						{
							hasAbstractMembers = true;
							break;
						}
					}
				}
				
				if (!hasAbstractMembers)
				{
					// class can be used as a base 
					// class even if it's abstract
					return; 
				}

				String message = String.Format("Argument '{0}' must be a concrete class", argumentName);
				
				throw new ArgumentException(message);
			}
		}
	}
}