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

namespace Castle.ActiveRecord.Generator
{
	using System;

	using Castle.ActiveRecord.Generator.Components;

	public delegate void ProjectDelegate(object sender, Project project);

	public delegate void ProjectReplaceDelegate(object sender, Project oldProject, Project newProject);

	/// <summary>
	/// Maintains all data related to a open project
	/// </summary>
	public class Model 
	{
		private Project _project;
		private String _filename;

		public Model()
		{
		}

		public string Filename
		{
			get { return _filename; }
			set { _filename = value; }
		}

		public Project CurrentProject
		{
			get { return _project; }
			set
			{
				_filename = null;

				if (OnProjectReplaced != null)
				{
					OnProjectReplaced(this, _project, value);
				}

				_project = value;
			}
		}

		public void Update()
		{
			if (OnProjectChanged != null)
			{
				OnProjectChanged(this, _project);
			}
		}

		public event ProjectDelegate OnProjectChanged;

		public event ProjectReplaceDelegate OnProjectReplaced;
	}
}
