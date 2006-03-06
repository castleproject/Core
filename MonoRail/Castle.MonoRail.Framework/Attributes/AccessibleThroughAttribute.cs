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

namespace Castle.MonoRail.Framework
{
	using System;

    /// <summary>
    /// Enum to identify a http verb 
    /// </summary>
    public enum Verb
    {
        /// <summary>
        /// The GET method means retrieve whatever information is identified by the Request-URI.
        /// <remarks>
        /// The convention has been established that the GET method SHOULD 
        /// NOT have the significance of taking an action other than retrieval. 
        /// </remarks>
        /// </summary>
        Get = 0,
        /// <summary>
        /// The POST method is used to request that the origin server accept the entity 
        /// enclosed in the request as a new subordinate of the resource identified by the 
        /// Request-URI in the Request-Line. 
        /// <remarks>
        /// The convention has been established that the POST method will
        /// take an action other than just retrieval. 
        /// </remarks>
        /// </summary>
        Post = 1
    }

	/// <summary>
	/// Decorates an action with a restriction to the HTTP method 
    /// that is allowed to request it.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method), Serializable]
	public class AccessibleThroughAttribute : Attribute
	{
		private readonly Verb verb;

        /// <summary>
        /// Constructs a AccessibleThroughAttribute with 
        /// the specified <paramref name="verb"/>.
        /// </summary>
        /// <param name="verb">The <see cref="Verb"/> to allow for this action.</param>
        public AccessibleThroughAttribute(Verb verb)
		{
            this.verb = verb;
		}

		/// <summary>
		/// The Verb to allow.
		/// </summary>
		public Verb Verb
		{
			get { return verb; }
		}
	}
}
