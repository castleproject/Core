// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.Applications.PestControl.Model
{
	using System;
	using System.Collections;

	/// <summary>
	/// Summary description for Projects.
	/// </summary>
	[Serializable]
	public class Project : Identifiable
	{
		public User _owner;
		public bool _isPublic;
		public String _name;
		public String _sourceControl;
		public String _buildSystem;

		public Project(bool _isPublic, string _name, string _sourceControl, string _buildSystem, User _owner)
		{
			this._owner = _owner;
			this._isPublic = _isPublic;
			this._name = _name;
			this._sourceControl = _sourceControl;
			this._buildSystem = _buildSystem;
		}

		public User Owner
		{
			get { return _owner; }
			set { _owner = value; }
		}

		public bool IsPublic
		{
			get { return _isPublic; }
			set { _isPublic = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public string SourceControl
		{
			get { return _sourceControl; }
			set { _sourceControl = value; }
		}

		public string BuildSystem
		{
			get { return _buildSystem; }
			set { _buildSystem = value; }
		}
	}

	/// <summary>
	/// Summary description for ProjectCollection.
	/// </summary>
	[Serializable]
	public class ProjectCollection : CollectionBase
	{
		public void Add(Project project)
		{
			lock(this.InnerList.SyncRoot)
			{
				InnerList.Add(project);
			}
		}
	}
}
