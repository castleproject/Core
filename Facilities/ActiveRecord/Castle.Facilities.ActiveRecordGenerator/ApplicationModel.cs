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

namespace Castle.Facilities.ActiveRecordGenerator
{
	using System;
	using System.Windows.Forms;

	using Castle.Facilities.ActiveRecordGenerator.Model;


	public class ApplicationModel : IApplicationModel
	{
		private IWin32Window _window;
		private Project _project;

		public ApplicationModel()
		{
		}

		#region IApplicationModel Members

		public Project CurrentProject
		{
			get { return _project; }
			set
			{
				if (OnProjectReplaced != null)
				{
					OnProjectReplaced(this, _project, value);
				}
				_project = value;
			}
		}

		public IWin32Window MainWindow
		{
			get { return _window; }
			set { _window = value; }
		}

		public void UpdateViews()
		{
			if (OnProjectChanged != null)
			{
				OnProjectChanged(this, _project);
			}
		}

		public event ProjectReplaceDelegate OnProjectReplaced;
		
		public event ProjectDelegate OnProjectChanged;

		#endregion
	}
}
