using System.Web;
// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Engine.Adapters
{
	using System;

	using Castle.MonoRail.Framework;
	
	public class ServerUtilityAdapter : IServerUtility
	{
		private readonly HttpServerUtility _server;

		public ServerUtilityAdapter(HttpServerUtility server)
		{
			_server = server;
		}

		public String HtmlEncode(String content)
		{
			return _server.HtmlEncode(content);
		}

		public String UrlEncode(String content)
		{
			return _server.UrlEncode(content);
		}

		public String UrlPathEncode(String content)
		{
			return _server.UrlPathEncode(content);
		}
	}
}
