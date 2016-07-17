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

#if FEATURE_NETCORE_REFLECTION_API

namespace System.Reflection
{
	using System.Linq;

	internal static class NetCoreReflectionExtensions
	{
		// .NET Core needs to expose GetConstructor that takes both flags and parameter types,
		// because we need to get the private constructor for a type with multiple constructors.
		// It should also provide the same for GetMethod, which luckily we don't need yet.
		public static ConstructorInfo GetConstructor(this Type type, BindingFlags bindingAttr, object binder, Type[] types, object[] modifiers)
		{
			if (binder != null) throw new NotSupportedException("Parameter binder must be null.");
			if (modifiers != null) throw new NotSupportedException("Parameter modifiers must be null.");

			return type.GetConstructors(bindingAttr)
				.SingleOrDefault(ctor => ctor.GetParameters().Select(p => p.ParameterType).SequenceEqual(types));
		}
	}
}

#endif