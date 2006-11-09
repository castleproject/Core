// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.Core
{
	using System;
	using System.Collections;

	/// <summary>
	/// Collection of <see cref="ConstructorCandidate"/>
	/// </summary>
	[Serializable]
	public class ConstructorCandidateCollection : ReadOnlyCollectionBase
	{
		private ConstructorCandidate fewerArgumentsCandidate;

		/// <summary>
		/// Adds the specified candidate.
		/// </summary>
		/// <param name="candidate">The candidate.</param>
		public void Add(ConstructorCandidate candidate)
		{
			if (fewerArgumentsCandidate == null)
			{
				fewerArgumentsCandidate = candidate;
			}
			else
			{
				if (candidate.Constructor.GetParameters().Length < 
					fewerArgumentsCandidate.Constructor.GetParameters().Length)
				{
					fewerArgumentsCandidate = candidate;
				}
			}

			InnerList.Add(candidate);
		}

		/// <summary>
		/// Gets the fewer arguments candidate.
		/// </summary>
		/// <value>The fewer arguments candidate.</value>
		public ConstructorCandidate FewerArgumentsCandidate
		{
			get { return fewerArgumentsCandidate; }
		}

		/// <summary>
		/// Clears this instance.
		/// </summary>
		public void Clear()
		{
			InnerList.Clear();
		}
	}
}
