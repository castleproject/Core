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

namespace Castle.MonoRail.ActiveRecordScaffold
{
	using System;
	
	using Castle.MonoRail.Framework;

	using Castle.Components.Common.TemplateEngine;
	using Castle.Components.Common.TemplateEngine.NVelocityTemplateEngine;


	public class ScaffoldingSupport : IScaffoldingSupport
	{
		public ScaffoldingSupport()
		{
		}

		public void Process(Controller controller)
		{
			ITemplateEngine templateEngine = null;

			lock(this)
			{
				if (templateEngine == null)
				{
					NVelocityTemplateEngine nvelTemplateEng = new NVelocityTemplateEngine();
#if DEBUG
					nvelTemplateEng.TemplateDir = @"E:\dev\projects\castle\MonoRail\Castle.MonoRail.ActiveRecordScaffold\Templates\";
					nvelTemplateEng.BeginInit();
					nvelTemplateEng.EndInit();
#else
					nvelTemplateEng.AssemblyName = "Castle.MonoRail.ActiveRecordScaffold";
					nvelTemplateEng.BeginInit();
					nvelTemplateEng.EndInit();
#endif

					templateEngine = nvelTemplateEng;
				}
			}

			ScaffoldingAttribute scaffoldAtt = controller.MetaDescriptor.Scaffolding;

			String name = scaffoldAtt.Model.Name;

			controller.CustomActions[ String.Format("new{0}", name) ] = new NewAction( scaffoldAtt.Model ); 
			controller.CustomActions[ String.Format("create{0}", name) ] = new CreateAction( scaffoldAtt.Model ); 
			controller.CustomActions[ String.Format("edit{0}", name) ] = new EditAction( scaffoldAtt.Model ); 
			controller.CustomActions[ String.Format("update{0}", name) ] = new UpdateAction( scaffoldAtt.Model ); 
			controller.CustomActions[ String.Format("remove{0}", name) ] = new RemoveAction( scaffoldAtt.Model ); 
			controller.CustomActions[ String.Format("confirm{0}", name) ] = new ConfirmRemoveAction( scaffoldAtt.Model ); 
			controller.CustomActions[ String.Format("list{0}", name) ] = new ListAction( scaffoldAtt.Model, templateEngine ); 
		}
	}
}