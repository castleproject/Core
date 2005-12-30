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

	using Castle.ActiveRecord;

	using Castle.Components.Common.TemplateEngine;

	using Castle.MonoRail.ActiveRecordSupport;
	using Castle.MonoRail.Framework;

	/// <summary>
	/// Performs the inclusion
	/// </summary>
	/// <remarks>
	/// Searchs for a template named <c>create{name}</c>
	/// </remarks>
	public class CreateAction : AbstractScaffoldAction
	{
		public CreateAction(Type modelType, ITemplateEngine templateEngine) : base(modelType, templateEngine)
		{
		}

		protected override string ComputeTemplateName(Controller controller)
		{
			return String.Format(@"{0}\create{1}", controller.Name, Model.Type.Name);
		}

		protected override void PerformActionProcess(Controller controller)
		{
			ARDataBinder binder = new ARDataBinder();

			object instance = binder.BindObject( Model.Type, "", controller.Params );

			SessionScope scope = new SessionScope();

			try
			{
				CommonOperationUtils.SaveInstance(instance, controller, errors, prop2Validation);

				scope.Dispose();

				controller.Redirect(controller.AreaName, controller.Name, "list" + Model.Type.Name);
			}
			catch(Exception ex)
			{
				errors.Add( "Could not save " + Model.Type.Name + ". " + ex.Message );

				scope.Dispose(true);
			}

			if (errors.Count != 0)
			{
				controller.Context.Flash["errors"] = errors;
				controller.Redirect(controller.AreaName, controller.Name, "new" + Model.Type.Name);
			}
		}

		protected override void RenderStandardHtml(Controller controller)
		{
			
		}
	}
}
