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

namespace Castle.MonoRail.Framework.Internal
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;

	/// <summary>
	/// Represents the cache output of a view component.
	/// To think: Should we consider enconding?
	/// </summary>
	[Serializable]
	public class ViewComponentCacheBag
	{
		private readonly StringBuilder content = new StringBuilder();
		private readonly IDictionary<string, object> contextEntries = new Dictionary<string, object>();

		/// <summary>
		/// Gets the cache writer.
		/// </summary>
		/// <value>The cache writer.</value>
		public TextWriter CacheWriter
		{
			get { return new StringWriter(content); }
		}

		/// <summary>
		/// Gets the content.
		/// </summary>
		/// <value>The content.</value>
		public string Content
		{
			get { return content.ToString(); }
		}

		/// <summary>
		/// Gets the context entries.
		/// </summary>
		/// <value>The context entries.</value>
		public IDictionary<string, object> ContextEntries
		{
			get { return contextEntries; }
		}
	}
}
