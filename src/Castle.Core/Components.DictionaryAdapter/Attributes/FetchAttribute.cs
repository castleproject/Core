﻿// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
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

	/// <summary>
	/// Identifies an interface or property to be pre-fetched.
	/// </summary>
	[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Property, AllowMultiple = false)]
	public class FetchAttribute : Attribute
	{
		/// <summary>
		/// Instructs fetching to occur.
		/// </summary>
		public FetchAttribute() : this(true)
		{
		}

		/// <summary>
		/// Instructs fetching according to <paramref name="fetch"/>
		/// </summary>
		public FetchAttribute(bool fetch)
		{
			Fetch = fetch;
		}

		/// <summary>
		/// Gets whether or not fetching should occur.
		/// </summary>
		public bool Fetch { get; private set; }
	}
}
