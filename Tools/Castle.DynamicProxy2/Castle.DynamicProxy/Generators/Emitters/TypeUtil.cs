// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Generators.Emitters
{
	using System;
	using System.Collections.Generic;

	public abstract class TypeUtil
	{
		/// <summary>
		/// Returns list of all unique interfaces implemented given types, including their base interfaces.
		/// </summary>
		/// <param name="types"></param>
		/// <returns></returns>
		public static ICollection<Type> GetAllInterfaces(params Type[] types)
		{
			if (types == null)
			{
				return Type.EmptyTypes;
			}

			var dummy = new object();
			// we should move this to HashSet once we no longer support .NET 2.0
			IDictionary<Type, object> interfaces = new Dictionary<Type, object>();
			foreach (var type in types)
			{
				if (type.IsInterface)
				{
					interfaces[type] = dummy;
				}
				foreach (var @interface in type.GetInterfaces())
				{
					interfaces[@interface] = dummy;
				}
			}
			return interfaces.Keys;
		}
	}
}
