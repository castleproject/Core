﻿// Copyright 2004-2015 Castle Project - http://www.castleproject.org/
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
	internal static class IntrospectionExtensions
	{
		// This allows us to use the new reflection API which separates Type and TypeInfo
		// while still supporting .NET 3.5 and 4.0.
		//
		// Return the System.Type for now, we will probably need to create a TypeInfo class
		// which inherits from Type like .NET 4.5 and implement the additional methods and
		// properties.
		public static Type GetTypeInfo(this Type type)
		{
			return type;
		}
	}
}

#endif