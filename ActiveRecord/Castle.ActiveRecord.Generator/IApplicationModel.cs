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
	using System.Windows.Forms;

	using Castle.ActiveRecord.Generator.Model;

	public enum ModelSelectionEnum
	{
		Table,
		Column,
		AllActiveRecord,
		ActiveRecord
	}
	
	public delegate void ProjectReplaceDelegate(object sender, Project oldProject, Project newProject);

	public delegate void ProjectDelegate(object sender, Project project);

	public delegate void SelectionDelegate(object sender, ModelSelectionEnum newSelection);


	public interface IApplicationModel
	{
		Project CurrentProject { get; set; }

		String SavedFileName { get; set; }

		TableDefinition CurrentTable { get; set; }

		ColumnDefinition CurrentColumn { get; set; }

		ActiveRecordDescriptor CurrentActiveRecord { get; set; }

		IWin32Window MainWindow { get; set; }

		event ProjectReplaceDelegate OnProjectReplaced;

		event ProjectDelegate OnProjectChanged;

		event SelectionDelegate OnSelectionChanged;

		void UpdateViews();
	}
}
