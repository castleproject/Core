// Copyright 2004-2015 Castle Project - http://www.castleproject.org/
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

#if FEATURE_LEGACY_REFLECTION_API

namespace System.Reflection
{
	using System.Collections.Generic;

	// This allows us to use the new reflection API while still supporting .NET 3.5 and 4.0.
	//
	// Methods like Attribute.IsDefined no longer exist in .NET Core so this provides a shim
	// for .NET 3.5 and 4.0.
	//
	// This class only implemented the required extensions so add more if needed in the order
	// from https://github.com/dotnet/corefx/blob/master/src/System.Reflection.Extensions/ref/System.Reflection.Extensions.cs
	internal static class CustomAttributeExtensions
	{
		public static IEnumerable<T> GetCustomAttributes<T>(this Assembly element) where T : Attribute
		{
			foreach (T a in Attribute.GetCustomAttributes(element, typeof(T)))
			{
				yield return a;
			}
		}

		public static IEnumerable<T> GetCustomAttributes<T>(this MemberInfo element, bool inherit) where T : Attribute
		{
			foreach (T a in Attribute.GetCustomAttributes(element, typeof(T), inherit))
			{
				yield return a;
			}
		}

		public static bool IsDefined(this MemberInfo element, Type attributeType)
		{
			return Attribute.IsDefined(element, attributeType);
		}

		public static bool IsDefined(this ParameterInfo element, Type attributeType)
		{
			return Attribute.IsDefined(element, attributeType);
		}
	}
}

#endif