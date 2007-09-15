// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Views
{
	using System;
	using System.IO;
	
	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// Represents a view source embedded as an assembly resource.
	/// </summary>
	public class EmbeddedResourceViewSource : IViewSource
	{
		private readonly AssemblySourceInfo sourceInfo;
		private readonly string templateName;

		/// <summary>
		/// Initializes a new instance of the <see cref="EmbeddedResourceViewSource"/> class.
		/// </summary>
		/// <param name="templateName">Name of the template.</param>
		/// <param name="sourceInfo">The source info.</param>
		public EmbeddedResourceViewSource(String templateName, AssemblySourceInfo sourceInfo)
		{
			this.templateName = templateName;
			this.sourceInfo = sourceInfo;
		}

		/// <summary>
		/// Opens the view stream.
		/// </summary>
		/// <returns></returns>
		public Stream OpenViewStream()
		{
			return sourceInfo.GetTemplateStream(templateName);
		}

		/// <summary>
		/// Gets the last modified.
		/// </summary>
		/// <value>The last modified.</value>
		public long LastModified
		{
			get { return DateTime.MinValue.Ticks; }
		}

		/// <summary>
		/// Gets or sets the last updated.
		/// </summary>
		/// <value>The last updated.</value>
		public long LastUpdated
		{
			get { return DateTime.MinValue.Ticks; }
			set { ; }
		}

		/// <summary>
		/// Gets a value indicating whether cache is enabled for it.
		/// </summary>
		/// <value><c>true</c> if cache is enabled for it; otherwise, <c>false</c>.</value>
		public bool EnableCache
		{
			get { return true; }
		}
	}
}
