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

namespace Castle.Facilities.ActiveRecordGenerator.Forms.Views
{
	using System;
	using System.Windows.Forms;

	using Castle.Facilities.ActiveRecordGenerator.Model;


	public class ProjectExplorerView
	{
		private TreeView _treeView;
		private IApplicationModel _model;

		public ProjectExplorerView(TreeView treeView, IApplicationModel model)
		{
			_model = model;
			_treeView = treeView;

			_model.OnProjectChanged += new ProjectDelegate(OnProjectChanged);
			_model.OnProjectReplaced += new ProjectReplaceDelegate(OnProjectReplaced);
		}

		protected void OnProjectChanged(object sender, Project project)
		{
			_treeView.Nodes.Clear();

			TreeNode catalog = _treeView.Nodes.Add( "Catalog" );
			catalog.ImageIndex = ImageConstants.Database_Catalog;
			catalog.SelectedImageIndex = ImageConstants.Database_Catalog;

			TreeNode classes = _treeView.Nodes.Add( "Classes" );
			classes.ImageIndex = ImageConstants.Classes_Entities;
			classes.SelectedImageIndex = ImageConstants.Classes_Entities;

			if(project.DatabaseDefinition != null)
			{
				TreeNode tables = catalog.Nodes.Add("Tables");
				tables.ImageIndex = ImageConstants.Database_Tables;
				tables.SelectedImageIndex = ImageConstants.Database_Tables;

				foreach(TableDefinition table in project.DatabaseDefinition.Tables)
				{
					TreeNode tableNode = tables.Nodes.Add( table.Name );
					tableNode.ImageIndex = ImageConstants.Database_Table;
					tableNode.SelectedImageIndex = ImageConstants.Database_Table;
				}
			}
		}

		protected void OnProjectReplaced(object sender, Project oldProject, Project newProject)
		{
			OnProjectChanged(sender, newProject);
		}
	}
}
