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
	using System.Collections;
	using System.Drawing;
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

			_treeView.AfterSelect += new TreeViewEventHandler(OnAfterSelect);

			_model.OnProjectChanged += new ProjectDelegate(OnProjectChanged);
			_model.OnProjectReplaced += new ProjectReplaceDelegate(OnProjectReplaced);
		}

		protected void OnProjectChanged(object sender, Project project)
		{
			_treeView.Nodes.Clear();

			ArrayList entities = new ArrayList();

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
					tableNode.Tag = table;

					foreach(ColumnDefinition column in table.Columns)
					{
						TreeNode columnNode = tableNode.Nodes.Add( column.Name );
						columnNode.ImageIndex = ImageConstants.Database_Field;
						columnNode.SelectedImageIndex = ImageConstants.Database_Field;
						columnNode.Tag = column;
					}

					if (table.RelatedDescriptor != null)
					{
						entities.Add(table.RelatedDescriptor);
					}
				}
			}

			foreach(ActiveRecordDescriptor desc in entities)
			{
				TreeNode arNode = classes.Nodes.Add( desc.ClassName );
				arNode.ImageIndex = ImageConstants.Classes_Entity;
				arNode.SelectedImageIndex = ImageConstants.Classes_Entity;
				arNode.Tag = desc;

				foreach(ActiveRecordPropertyDescriptor property in desc.Properties)
				{
					TreeNode propertyNode = arNode.Nodes.Add( property.PropertyName );
					propertyNode.ImageIndex = ImageConstants.Classes_Property;
					propertyNode.SelectedImageIndex = ImageConstants.Classes_Property;
					
					TreeNode fieldNode = arNode.Nodes.Add( property.PropertyFieldName );
					fieldNode.ImageIndex = ImageConstants.Classes_Private_Field;
					fieldNode.SelectedImageIndex = ImageConstants.Classes_Private_Field;
				}
			}

			_treeView.ExpandAll();
		}

		protected void OnProjectReplaced(object sender, Project oldProject, Project newProject)
		{
			OnProjectChanged(sender, newProject);
		}

		private void OnAfterSelect(object sender, TreeViewEventArgs e)
		{
			_treeView.ContextMenu.MenuItems[0].Enabled = (e.Node.Tag is TableDefinition);

			if (e.Node.Tag == null)
			{
				_model.CurrentActiveRecord = null;
				_model.CurrentTable = null;
				_model.CurrentColumn = null;
			}

			if (e.Node.Tag is TableDefinition)
			{
				_model.CurrentTable = e.Node.Tag as TableDefinition;
			}
			else if (e.Node.Tag is ColumnDefinition)
			{
				_model.CurrentColumn = e.Node.Tag as ColumnDefinition;
			}
			else if (e.Node.Tag is ActiveRecordDescriptor)
			{
				_model.CurrentActiveRecord = e.Node.Tag as ActiveRecordDescriptor;
			}
		}
	}
}
