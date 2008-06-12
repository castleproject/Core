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
// limitations under the License.

namespace Castle.MonoRail.Framework.Providers
{
	using System.Reflection;

	/// <summary>
	/// Contains the method pair for an async action
	/// </summary>
	public class AsyncActionPair
	{
		private readonly string name;
		private readonly MethodInfo beginActionInfo;
		private readonly MethodInfo endActionInfo;

		/// <summary>
		/// Gets the begin action info.
		/// </summary>
		/// <value>The begin action info.</value>
		public MethodInfo BeginActionInfo
		{
			get { return beginActionInfo; }
		}

		/// <summary>
		/// Gets the end action info.
		/// </summary>
		/// <value>The end action info.</value>
		public MethodInfo EndActionInfo
		{
			get { return endActionInfo; }
		}

		/// <summary>
		/// Gets the name of this action
		/// </summary>
		/// <value>The name.</value>
		public string Name
		{
			get { return name; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncActionPair"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="beginActionInfo">The begin action info.</param>
		/// <param name="endActionInfo">The end action info.</param>
		public AsyncActionPair(string name, MethodInfo beginActionInfo, MethodInfo endActionInfo)
		{
			this.name = name;
			this.beginActionInfo = beginActionInfo;
			this.endActionInfo = endActionInfo;
		}


		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		public override string ToString()
		{
			return Name;
		}
	}
}