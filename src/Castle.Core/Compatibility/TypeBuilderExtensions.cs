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
	using System.Reflection.Emit;

	// This allows us to use the new reflection API while still supporting .NET 3.5 and 4.0.
	internal static class TypeBuilderExtensions
	{
		// TypeBuilder and GenericTypeParameterBuilder no longer inherit from Type but TypeInfo,
		// so there is now an AsType method to get the Type which we are providing here to shim to itself.
		public static Type AsType(this TypeBuilder builder)
		{
			return builder;
		}

		public static Type AsType(this GenericTypeParameterBuilder builder)
		{
			return builder;
		}
	}
}

#endif