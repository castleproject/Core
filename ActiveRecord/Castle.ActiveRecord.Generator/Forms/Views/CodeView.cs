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

namespace Castle.ActiveRecord.Generator.Forms.Views
{
	using System;
	using System.Windows.Forms;


	public class CodeView
	{
		private RichTextBox _codeBox;
		private IApplicationModel _model;

		public CodeView(RichTextBox codeBox, IApplicationModel model)
		{
			_codeBox = codeBox;
			_codeBox.Text = "Select an ActiveRecord on the tree.";

			_model = model;

			_model.OnSelectionChanged += new SelectionDelegate(OnSelectionChanged);
		}

		private void OnSelectionChanged(object sender, ModelSelectionEnum newSelection)
		{
			if (newSelection == ModelSelectionEnum.ActiveRecord && _model.CurrentActiveRecord != null)
			{
				_codeBox.Text = _model.CurrentProject.GenerateCode(_model.CurrentActiveRecord);
			}
		}
	}
}
