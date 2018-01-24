// Copyright 2004-2016 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Generators
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.Threading;

	public static class AttributesToAvoidReplicating
	{
		private static IList<Type> attributes;

		static AttributesToAvoidReplicating()
		{
			attributes = new List<Type>()
			{
				typeof(System.Runtime.InteropServices.ComImportAttribute),
				typeof(System.Runtime.InteropServices.MarshalAsAttribute),
#if !DOTNET35
				typeof(System.Runtime.InteropServices.TypeIdentifierAttribute),
#endif
#if FEATURE_SECURITY_PERMISSIONS
				typeof(System.Security.Permissions.SecurityAttribute),
#endif
			};
		}

		public static void Add(Type attribute)
		{
			// notes:
			// this class is made thread-safe by replacing the backing list rather than adding to it
			// this loop runs until the attributes collection contains the new attribute
			// it is necessary to ensure that if multiple threads concurrently replace the list that they
			// don't override each others changes
			IList<Type> originalAttributes;
			while (!(originalAttributes = attributes).Contains(attribute))
			{
				IList<Type> newAttributes = new List<Type>(originalAttributes) { attribute };

				Interlocked.CompareExchange(ref attributes, newAttributes, originalAttributes);
			};
		}

		public static void Add<T>()
		{
			Add(typeof(T));
		}

		public static bool Contains(Type attribute)
		{
			return attributes.Contains(attribute);
		}

		internal static bool ShouldAvoid(Type attribute)
		{
			return attributes.Any(attr => attr.GetTypeInfo().IsAssignableFrom(attribute.GetTypeInfo()));
		}
	}
}