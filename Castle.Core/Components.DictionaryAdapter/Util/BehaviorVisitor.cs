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
	using System.Collections.Generic;

	public class BehaviorVisitor
	{
		private List<KeyValuePair<Func<object, bool>, Func<object, bool>>> conditions;

		public BehaviorVisitor()
		{
			conditions = new List<KeyValuePair<Func<object,bool>, Func<object, bool>>>();
		}

		public BehaviorVisitor OfType<T>(Func<T, bool> apply)
		{
			return AddCondition(b => b is T, b => apply((T)b));
		}

		public BehaviorVisitor OfType<T>(Action<T> apply)
		{
			return AddCondition(b => b is T, b => { apply((T)b); return true; }); 
		}

		public BehaviorVisitor Match<T>(Func<T, bool> match, Func<T, bool> apply)
		{
			return AddCondition(b => b is T && match((T)b), b => apply((T)b));
		}

		public BehaviorVisitor Match<T>(Func<T, bool> match, Action<T> apply)
		{
			return AddCondition(b => b is T && match((T)b), b => { apply((T)b); return true; });
		}

		public BehaviorVisitor Match(Func<object, bool> match, Func<object, bool> apply)
		{
			return Match<object>(match, apply);
		}

		public BehaviorVisitor Match(Func<object, bool> match, Action<object> apply)
		{
			return Match<object>(match, apply);
		}

		public void Apply(IEnumerable behaviors)
		{
			foreach (var behavior in behaviors)
			foreach (var condition in conditions)
			{
				if (condition.Key(behavior))
				{
					condition.Value(behavior);
				}
			}
		}

		private BehaviorVisitor AddCondition(Func<object, bool> predicate, Func<object, bool> action)
		{
			conditions.Add(new KeyValuePair<Func<object, bool>, Func<object, bool>>(predicate, action));
			return this;
		}
	}
}
