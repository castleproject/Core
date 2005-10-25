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
	using System.Reflection;
	using System.Collections;

	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Helpers;

	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework.Internal;

	using Castle.Components.Common.TemplateEngine;


	public abstract class AbstractScaffoldAction : IDynamicAction
	{
		protected static readonly object[] Empty = new object[0];

		protected readonly Type modelType;
		protected readonly HtmlHelper helper = new HtmlHelper();
		protected readonly ITemplateEngine templateEngine;
		
		protected PropertyInfo keyProperty;
		protected IDictionary prop2Validation = new Hashtable();

		private ActiveRecordModel model;

		public AbstractScaffoldAction( Type modelType, ITemplateEngine templateEngine )
		{
			this.modelType = modelType;
			this.templateEngine = templateEngine;
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
			ActiveRecordModel model = DomainModel.GetModel( modelType );
	
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
			PrimaryKeyModel keyModel = ObtainPKProperty(model);
			
			if (keyModel != null) keyProperty = keyModel.Property;
		}

		protected static PrimaryKeyModel ObtainPKProperty(ActiveRecordModel model)
		{
			if (model == null) return null;

			ActiveRecordModel curModel = model;

			while (curModel != null)
			{
				foreach(PrimaryKeyModel keyModel in curModel.Ids)
				{
					return keyModel;
				}

				curModel = curModel.Parent;
			}

			return null;
		}
	}
}
