// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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
// limitations under the License.;

namespace Castle.Facilities.WcfIntegration
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	internal static class WcfUtils
	{
		public static bool FindDependency<T>(IDictionary dependencies,
										     out T match)
		{
			return FindDependency<T>(dependencies, t => true, out match);
		}

		public static bool FindDependency<T>(IDictionary dependencies, 
			                                 Predicate<T> test, out T match)
		{
			match = default(T);

			foreach (object dependency in dependencies.Values)
			{
				if (dependency is T)
				{
					T candidate = (T)dependency;

					if (test(candidate))
					{
						match = candidate;
						return true;
					}
				}
			}
			return false;
		}

		public static bool FindDependency<T>(ICollection<T> dependencies,
										     out T match)
		{
			return FindDependency<T>(dependencies, t => true, out match);
		}

		public static bool FindDependency<T>(ICollection<T> dependencies,
											 Predicate<T> test, out T match)
		{
			match = default(T);

			foreach (object dependency in dependencies)
			{
				if (dependency is T)
				{
					T candidate = (T)dependency;

					if (test(candidate))
					{
						match = candidate;
						return true;
					}
				}
			}
			return false;
		}
	}
}
