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

	using Castle.Model;

	using Castle.Facilities.ActiveRecordGenerator.Forms;
	using Castle.Facilities.ActiveRecordGenerator.Action;



	public class ApplicationController : IApplicationController, IStartable
	{
		private MainForm _mainForm;
		private IActionFactory _actionFactory;
		private IApplicationModel _model;

		public ApplicationController(MainForm mainForm, IApplicationModel model)
		{
			_mainForm = mainForm;
			_model = model;

			_mainForm.OnAction += new ActionDelegate(OnAction);
		}

		#region IApplicationController Members

		public IActionFactory ActionFactory
		{
			get { return _actionFactory; }
			set { _actionFactory = value; }
		}

		#endregion

		#region IStartable Members

		public void Start()
		{
			Application.Run(_mainForm);
		}

		public void Stop()
		{

		}

		#endregion

		protected void OnAction(String actionName)
		{
			IAction action = _actionFactory.Create(actionName);

			action.Execute(_model);
		}
	}
}
