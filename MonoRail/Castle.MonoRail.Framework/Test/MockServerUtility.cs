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

namespace Castle.MonoRail.Framework.Test
{
	using System;
	using System.IO;

	public class MockServerUtility : IServerUtility
	{
		public virtual string MapPath(string virtualPath)
		{
			return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, virtualPath);
		}

		public virtual string HtmlEncode(string content)
		{
			return content;
		}

		public virtual string UrlEncode(string content)
		{
			return content.Replace("&", "&amp;");
		}

		public virtual string UrlDecode(string content)
		{
			return content.Replace("&amp", "&");
		}

		public virtual string UrlPathEncode(string content)
		{
			throw new NotImplementedException();
		}

		public virtual string JavaScriptEscape(string content)
		{
			throw new NotImplementedException();
		}
	}
}
