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

namespace Castle.MonoRail.Framework.Views.NVelocity
{
	using System;

	public sealed class TemplateKeys
	{
		private TemplateKeys() { throw new InvalidOperationException(); }
	
		public static readonly String LayoutPath   = "layouts";
		public static readonly String ChildContent = "childContent";
		public static readonly String Context      = "context";
		public static readonly String Request      = "request";
		public static readonly String Response     = "response";
		public static readonly String Session      = "session";
		public static readonly String Controller   = "controller";
		public static readonly String SiteRoot     = "siteroot";
	}
}
