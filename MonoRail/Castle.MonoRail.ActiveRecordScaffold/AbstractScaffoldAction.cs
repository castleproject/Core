using System.Reflection;
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
	using System.Collections;

	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Helpers;

	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework.Internal;


	public abstract class AbstractScaffoldAction : IDynamicAction
	{
		protected readonly Type modelType;
		protected readonly HtmlHelper helper = new HtmlHelper();
		
		protected PropertyInfo keyProperty;
		protected IDictionary prop2Validation = new Hashtable();

		private ActiveRecordModel model;

		public AbstractScaffoldAction( Type modelType )
		{
			this.modelType = modelType;
		}

		public void Execute(Controller controller)
		{
			model = GetARModel();

			helper.SetController(controller);

			SetDefaultLayout(controller);

			PerformActionProcess(controller);

			String templateName = ComputeTemplateName(controller);

			if (controller.HasTemplate(templateName))
			{
				controller.RenderView(controller.Name, templateName);
			}
			else
			{
				RenderStandardHtml( controller );
			}
		}

		protected abstract string ComputeTemplateName(Controller controller);

		protected abstract void PerformActionProcess(Controller controller);

		protected abstract void RenderStandardHtml(Controller controller);

		protected ActiveRecordModel Model
		{
			get { return model; }
		}

		protected ActiveRecordModel GetARModel()
		{
			ActiveRecordModel model = ActiveRecordBase._GetModel( modelType );
	
			if (model == null)
			{
				throw new ScaffoldException("Specified type isn't an ActiveRecord type or the ActiveRecord " + 
					"framework wasn't started properly. Did you forget about the Initialize method?");
			}

			return model;
		}

		protected void SetDefaultLayout(Controller controller)
		{
			if (controller.LayoutName == null)
			{
				controller.LayoutName = "scaffold";
			}
		}

		protected void ObtainPKProperty()
		{
			ActiveRecordModel curModel = model;

			while (keyProperty == null && curModel != null)
			{
				foreach(PrimaryKeyModel keyModel in curModel.Ids)
				{
					keyProperty = keyModel.Property;
				}

				curModel = curModel.Parent;
			}
		}
	}
}
