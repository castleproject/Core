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
	using System.Collections.Generic;
	using System.Diagnostics;

	public class NamingScope : INamingScope
	{
		private readonly IDictionary<string, int> names = new Dictionary<string, int>();
		private readonly INamingScope parentScope;

		public NamingScope()
		{
		}

		private NamingScope(INamingScope parent)
		{
			parentScope = parent;
		}

		public INamingScope ParentScope
		{
			get { return parentScope; }
		}

		public string GetUniqueName(string suggestedName)
		{
			Debug.Assert(string.IsNullOrEmpty(suggestedName) == false,
			             "string.IsNullOrEmpty(suggestedName) == false");

			int counter;
			if (!names.TryGetValue(suggestedName, out counter))
			{
				names.Add(suggestedName, 0);
				return suggestedName;
			}

			counter++;
			names[suggestedName] = counter;
			return suggestedName + "_" + counter.ToString();
		}

		public INamingScope SafeSubScope()
		{
			return new NamingScope(this);
		}
	}
}