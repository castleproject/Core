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

namespace Castle.MonoRail.Views.AspView.VCompile
{
	public class Arguments
	{
		private bool _wait;
		private string _siteRoot;
		private string _viewPathRoot;

		public bool Wait
		{
			get { return _wait; }
			set { _wait = value; }
		}

		public string SiteRoot
		{
			get { return _siteRoot; }
			set { _siteRoot = value; }
		}

		public string ViewPathRoot
		{
			get { return _viewPathRoot; }
			set { _viewPathRoot = value; }
		}
	}
}