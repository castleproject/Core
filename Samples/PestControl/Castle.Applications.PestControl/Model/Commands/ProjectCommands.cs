using System.Collections;
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

namespace Castle.Applications.PestControl.Model
{
	using System;

	using Bamboo.Prevalence;

	/// <summary>
	/// Summary description for ProjectCommands.
	/// </summary>
	[Serializable]
	public class CreateProjectCommand : ICommand
	{
		bool _isPublic; 
		string _name; 
		string _sourceControl; 
		string _buildSystem; 
		String _ownerEmail;
		IDictionary _sourceControlProperties;

		public CreateProjectCommand(bool _isPublic, string _name, string _sourceControl, 
			string _buildSystem, string _ownerEmail, IDictionary _sourceControlProperties)
		{
			this._isPublic = _isPublic;
			this._name = _name;
			this._sourceControl = _sourceControl;
			this._buildSystem = _buildSystem;
			this._ownerEmail = _ownerEmail;
			this._sourceControlProperties = _sourceControlProperties;
		}

		public object Execute(object system)
		{
			PestControlModel model = (PestControlModel) system;
			User owner = model.Users.FindByEmail(_ownerEmail);

			Project project = new Project(_isPublic, _name, _sourceControl, _buildSystem, owner);

			foreach(DictionaryEntry entry in _sourceControlProperties)
			{
				project.SourceControlProperties.Add(entry.Key, entry.Value);
			}

			model.Projects.Add( project );

			return project;
		}
	}
}
