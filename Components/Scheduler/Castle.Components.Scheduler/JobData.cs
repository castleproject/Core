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

namespace Castle.Components.Scheduler
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Job data persists job parameters and state across job executions.
	/// The contents of the job data must be serializable if they are to be
	/// used with a persistent store.
	/// </summary>
	[Serializable]
	public class JobData : ICloneable<JobData>
	{
		private IDictionary<string, object> state;

		/// <summary>
		/// Create an empty job data structure.
		/// </summary>
		public JobData()
		{
		}

		/// <summary>
		/// Creates a job data structure with the specified initial state.
		/// </summary>
		/// <param name="state">The initial state</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="state"/> is null</exception>
		public JobData(IDictionary<string, object> state)
		{
			if (state == null)
				throw new ArgumentNullException("state");

			this.state = state;
		}

		/// <summary>
		/// Gets a dictionary of state values.
		/// </summary>
		public IDictionary<string, object> State
		{
			get
			{
				if (state == null)
					state = new Dictionary<string, object>();
				return state;
			}
		}

		/// <summary>
		/// Clones the job data including a shallow copy of the state dictionary.
		/// </summary>
		/// <returns>The cloned job data</returns>
		public virtual JobData Clone()
		{
			JobData clone = state != null ? new JobData(new Dictionary<string, object>(state)) : new JobData();

			return clone;
		}

		object ICloneable.Clone()
		{
			return Clone();
		}
	}
}