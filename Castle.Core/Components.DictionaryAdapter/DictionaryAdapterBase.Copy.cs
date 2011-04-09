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

namespace Castle.Components.DictionaryAdapter
{
	using System;
	using System.Linq;

	public abstract partial class DictionaryAdapterBase
	{
		public void CopyTo(IDictionaryAdapter other)
		{
			CopyTo(other, null);
		}

		public void CopyTo(IDictionaryAdapter other, Predicate<PropertyDescriptor> selector)
		{
			if (Meta.Type.IsAssignableFrom(other.Meta.Type) == false)
			{
				throw new ArgumentException(string.Format(
					"Unable to copy to {0}.  The type must be assignable from {1}.",
					other.Meta.Type.FullName, Meta.Type.FullName));
			}

			if (This.CopyStrategy != null && This.CopyStrategy.Copy(this, other, ref selector))
			{
				return;
			}

			selector = selector ?? (p => true);

			foreach (var property in This.Properties.Values.Where(p => selector(p)))
			{
				var propertyValue = GetProperty(property.PropertyName, true);
				other.SetProperty(property.PropertyName, ref propertyValue);
			}
		}
	}
}
