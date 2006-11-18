// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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
	using System.Reflection;
	using Castle.ActiveRecord;
	using Castle.Components.Common.TemplateEngine;
	using Castle.MonoRail.Framework;

	/// <summary>
	/// Performs the update
	/// </summary>
	/// <remarks>
	/// Searchs for a template named <c>create{name}</c>
	/// </remarks>
	public class UpdateAction : AbstractScaffoldAction
	{
		public UpdateAction(Type modelType, ITemplateEngine templateEngine, bool useModelName, bool useDefaultLayout) : 
			base(modelType, templateEngine, useModelName, useDefaultLayout)
		{
		}

		protected override string ComputeTemplateName(Controller controller)
		{
			return String.Format(@"{0}\update{1}", controller.Name, Model.Type.Name);
		}

		protected override void PerformActionProcess(Controller controller)
		{
			object instance = null;
			
			try
			{
				AssertIsPost(controller);
				
				instance = binder.BindObject(Model.Type, Model.Type.Name, builder.BuildSourceNode(controller.Request.Form));

				CommonOperationUtils.SaveInstance(instance, controller, errors, ref prop2Validation, false);

				SessionScope.Current.Flush();
				
				if (UseModelName)
				{
					controller.Redirect(controller.AreaName, controller.Name, "list" + Model.Type.Name);
				}
				else
				{
					controller.Redirect(controller.AreaName, controller.Name, "list");
				}
			}
			catch(Exception ex)
			{
				errors.Add("Could not save " + Model.Type.Name + ". " + ex.Message);
			}

			if (errors.Count != 0)
			{
				controller.Context.Flash[Model.Type.Name] = instance;
				controller.Context.Flash["errors"] = errors;

				PropertyInfo keyProp = ObtainPKProperty();
				IDictionary props = new Hashtable();

				if (instance != null)
				{
					props[keyProp.Name] = keyProp.GetValue(instance, null);
				}
				
				if (UseModelName)
				{
					controller.Redirect(controller.AreaName, controller.Name, "edit" + Model.Type.Name, props);
				}
				else
				{
					controller.Redirect(controller.AreaName, controller.Name, "edit", props);
				}
			}
		}

		protected override void RenderStandardHtml(Controller controller)
		{
		}
	}
}