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

namespace Castle.Applications.PestControl.Web.Controllers
{
	using System;
	using System.Collections;
	using Bamboo.Prevalence;
	using Castle.Model;
	using Castle.CastleOnRails.Framework;
	using Castle.Applications.PestControl.Model;
	using Castle.Applications.PestControl.Services.BuildSystems;
	using Castle.Applications.PestControl.Services.SourceControl;

	/// <summary>
	/// Summary description for ProjectController.
	/// </summary>
	[Transient]
	public class ProjectController : SmartDispatcherController
	{
		private PestControlModel _model;
		private SourceControlManager _scm;
		private BuildSystemManager _bsm;
		private PrevalenceEngine _engine;

		public ProjectController(
			PestControlModel model, PrevalenceEngine engine,
			SourceControlManager scm, BuildSystemManager bsm)
		{
			_engine = engine;
			_model = model;
			_scm = scm;
			_bsm = bsm;
		}

		public void New()
		{
			PropertyBag.Add("SourceControlManager", _scm);
			PropertyBag.Add("BuildSystemManager", _bsm);
		}

		public void Insert(String name, String bs, String sc)
		{
			bool isPublic = (bool) PropertyBag["isPublic"];
			String ownerEmail = (String) PropertyBag["ownerEmail"];
			Hashtable properties = (Hashtable) PropertyBag["properties"];

			_engine.ExecuteCommand(
				new CreateProjectCommand(isPublic, name, sc, bs, ownerEmail, properties));

			Redirect("dashboard", "index");
		}
	}
}