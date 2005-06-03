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

namespace AspectSharp
{
	using System;

	/// <summary>
	/// Summary description for AssertUtil.
	/// </summary>
	internal abstract class AssertUtil
	{
		public static void NotNull(object value, String name)
		{
			if (value == null)
			{
				throw new ApplicationException(
					String.Format("Instance field or property {0} can't be null", name));
			}
		}

		public static void ArgumentNotNull(object argValue, String argName)
		{
			if (argValue == null)
			{
				throw new ArgumentNullException(
					argName,
					String.Format("Argument {0} can't be null", argName));
			}
		}

		public static void ArgumentNotInterface(Type argValue, String argName)
		{
			if (argValue.IsInterface || argValue.IsAbstract)
			{
				throw new ArgumentNullException(
					argName,
					String.Format("Argument {0} can't be an interface", argName));
			}
		}

		public static void ArgumentIsInterface(Type[] args, String argName)
		{
			foreach(Type type in args)
			{
				ArgumentIsInterface(type, argName);
			}
		}

		public static void ArgumentIsInterface(Type argValue, String argName)
		{
			if (!argValue.IsInterface)
			{
				throw new ArgumentNullException(
					argName,
					String.Format("Argument {0} must be an interface", argName));
			}
		}
	}
}