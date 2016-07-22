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
	using System.Collections;

	/// <summary>
	/// Removes a property if null or empty string, guid or collection.
	/// </summary>
	public class RemoveIfEmptyAttribute : RemoveIfAttribute
	{
		public RemoveIfEmptyAttribute()
			: base(RemoveIfEmptyCondition.Instance)
		{
		}

		private new Type Condition { get; set; }

		class RemoveIfEmptyCondition : ICondition
		{
			public static readonly RemoveIfEmptyCondition Instance = new RemoveIfEmptyCondition();

			public bool SatisfiedBy(object value)
			{
				return value == null ||
					   IsEmptyString(value) ||
					   IsEmptyGuid(value) ||
					   IsEmptyCollection(value);
			}

			private static bool IsEmptyString(object value)
			{
				return (value is string && ((string)value).Length == 0);
			}

			private static bool IsEmptyGuid(object value)
			{
				return (value is Guid && ((Guid)value) == Guid.Empty);
			}

			private static bool IsEmptyCollection(object value)
			{
				return (value is IEnumerable && ((IEnumerable)value).GetEnumerator().MoveNext() == false);
			}
		}
	}
}