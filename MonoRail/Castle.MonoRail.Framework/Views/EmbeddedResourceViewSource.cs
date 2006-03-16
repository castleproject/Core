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

namespace Castle.MonoRail.Framework.Views
{
	using System;
	using System.IO;
	using Castle.MonoRail.Framework.Internal;


	public class EmbeddedResourceViewSource : IViewSource
	{
		private readonly AssemblySourceInfo sourceInfo;
		private readonly string templateName;

		public EmbeddedResourceViewSource(String templateName, AssemblySourceInfo sourceInfo)
		{
			this.templateName = templateName;
			this.sourceInfo = sourceInfo;
		}

		public Stream OpenViewStream()
		{
			return sourceInfo.GetTemplateStream(templateName);
		}

		public long LastModified
		{
			get { return DateTime.MinValue.Ticks; }
		}

		public long LastUpdated
		{
			get { return DateTime.MinValue.Ticks; }
			set { ; }
		}

		public bool EnableCache
		{
			get { return true; }
		}
	}
}
