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
		private TableDefinition _table;
		private ColumnDefinition _column;
		private ActiveRecordDescriptor _arDescriptor;

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

		public TableDefinition CurrentTable
		{
			get { return _table; }
			set
			{
				_table = value;

				if (OnSelectionChanged != null)
				{
					OnSelectionChanged(this, ModelSelectionEnum.Table);
				}
			}
		}

		public ColumnDefinition CurrentColumn
		{
			get { return _column; }
			set
			{
				_column = value;
			
				if (OnSelectionChanged != null)
				{
					OnSelectionChanged(this, ModelSelectionEnum.Column);
				}
			}
		}

		public ActiveRecordDescriptor CurrentActiveRecord
		{
			get { return _arDescriptor; }
			set
			{
				_arDescriptor = value;

				if (OnSelectionChanged != null)
				{
					OnSelectionChanged(this, ModelSelectionEnum.ActiveRecord);
				}
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

		public event SelectionDelegate OnSelectionChanged;

		#endregion
	}
}
