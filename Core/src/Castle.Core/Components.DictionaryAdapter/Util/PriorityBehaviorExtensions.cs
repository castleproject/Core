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
	using System.Collections.Generic;
	using System.Linq;

	static class PriorityBehaviorExtensions
	{
		class PriorityBehavior<T> : IComparable<PriorityBehavior<T>> where T : IDictionaryBehavior
		{
			public PriorityBehavior(T behavior, int priority)
			{
				Behavior = behavior;
				Priority = priority;
			}

			public T Behavior { get; private set; }

			public int Priority { get; private set; }

			public int CompareTo(PriorityBehavior<T> other)
			{
				if (Behavior.ExecutionOrder == other.Behavior.ExecutionOrder)
					return Priority - other.Priority;
				return Behavior.ExecutionOrder > other.Behavior.ExecutionOrder ? 1 : -1;
			}
		}

		public static IEnumerable<T> Prioritize<T>(this IEnumerable<T> first, params IEnumerable<T>[] remaining)
			where T : IDictionaryBehavior
		{
			var allBehaviors = new[] { first }.Union(remaining).Where(behaviors => behaviors != null && behaviors.Any()).ToArray();

			switch (allBehaviors.Length)
			{
				case 0:
					return Enumerable.Empty<T>();
				case 1:
					return allBehaviors[0].OrderBy(behavior => behavior.ExecutionOrder);
				default:
					return allBehaviors.SelectMany((bs, priority) => bs.Select(b => new PriorityBehavior<T>(b, priority)))
						.OrderBy(priority => priority).Select(priority => priority.Behavior);;
			}
		}
	}
}
