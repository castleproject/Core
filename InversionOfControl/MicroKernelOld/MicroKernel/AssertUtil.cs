// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.MicroKernel
{
	using System;

	/// <summary>
	/// Summary description for AssertUtil.
	/// </summary>
	public abstract class AssertUtil
	{
		public static void ArgumentNotNull( object argValue, String argName )
		{
			if (argValue == null)
			{
				throw new ArgumentNullException(
					argName, 
					String.Format("Argument {0} can't be null", argName) );
			}
		}

		public static void ArgumentMustBeInterface( Type argValue, String argName )
		{
			if (argValue != null && !argValue.IsInterface)
			{
				throw new ArgumentNullException(
					argName, 
					String.Format("Argument {0} must be an interface", argName) );
			}
		}

		public static void ArgumentMustNotBeInterface( Type argValue, String argName )
		{
			if (argValue != null && argValue.IsInterface)
			{
				throw new ArgumentNullException(
					argName, 
					String.Format("Argument {0} can't be an interface", argName) );
			}
		}

		public static void ArgumentMustNotBeAbstract( Type argValue, String argName )
		{
			if (argValue != null && argValue.IsAbstract)
			{
				throw new ArgumentNullException(
					argName, 
					String.Format("Argument {0} can't be abstract", argName) );
			}
		}
	}
}
