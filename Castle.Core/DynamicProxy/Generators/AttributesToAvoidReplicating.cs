// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
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
	using System.Runtime.InteropServices;
	using System.Security.Permissions;

	public static class AttributesToAvoidReplicating
	{
		private static readonly IList<Type> attributes = new List<Type>();

		static AttributesToAvoidReplicating()
		{
			Add<ComImportAttribute>();
#if !SILVERLIGHT
			Add<SecurityPermissionAttribute>();
#endif
#if DOTNET40
			Add<TypeIdentifierAttribute>();
#endif
		}

		public static void Add(Type attribute)
		{
			if (attributes.Contains(attribute) == false)
			{
				attributes.Add(attribute);
			}
		}

		public static void Add<T>()
		{
			Add(typeof(T));
		}

		public static bool Contains(Type type)
		{
			return attributes.Contains(type);
		}
	}
}