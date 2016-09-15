// Copyright 2004-2014 Castle Project - http://www.castleproject.org/
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

namespace Castle.Core.Internal
{
	using System;
	using System.Reflection;

	internal static class TypeExtensions
	{
		/// <summary>
		/// Find the best available name to describe a type.
		/// </summary>
		/// <remarks>
		/// Usually the best name will be <see cref="Type.FullName"/>, but
		/// sometimes that's null (see http://msdn.microsoft.com/en-us/library/system.type.fullname%28v=vs.110%29.aspx)
		/// in which case the method falls back to <see cref="MemberInfo.Name"/>.
		/// </remarks>
		/// <param name="type">the type to name</param>
		/// <returns>the best name</returns>
		public static string GetBestName(this Type type)
		{
			return type.FullName ?? type.Name;
		}
	}
}
