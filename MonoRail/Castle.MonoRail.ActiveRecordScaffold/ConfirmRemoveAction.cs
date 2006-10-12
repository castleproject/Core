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

	using Castle.ActiveRecord;
	using Castle.Components.Common.TemplateEngine;
	using Castle.MonoRail.Framework;


	/// <summary>
	/// Displays a confirmation message before performing 
	/// the removal of the instance
	/// </summary>
	/// <remarks>
	/// Searchs for a template named <c>confirm{name}remove</c>
	/// </remarks>
	public class ConfirmRemoveAction : AbstractScaffoldAction
	{
		public ConfirmRemoveAction(Type modelType, ITemplateEngine templateEngine, bool useModelName, bool useDefaultLayout) : 
			base(modelType, templateEngine, useModelName, useDefaultLayout)
		{
		}

		protected override string ComputeTemplateName(Controller controller)
		{
			return String.Format(@"{0}\confirm{1}remove", controller.Name, Model.Type.Name);
		}

		protected override void PerformActionProcess(Controller controller)
		{
			base.PerformActionProcess(controller);
			
			object idVal = CommonOperationUtils.ReadPkFromParams(controller, ObtainPKProperty());

			try
			{
				object instance = ActiveRecordMediator.FindByPrimaryKey(Model.Type, idVal, true);

				controller.PropertyBag["instance"] = instance;
				controller.PropertyBag["id"] = idVal;
			}
			catch(Exception ex)
			{
				throw new ScaffoldException("Could not obtain instance by using this id", ex);
			}
		}

		protected override void RenderStandardHtml(Controller controller)
		{
			SetUpHelpers(controller);
			RenderFromTemplate("confirm.vm", controller);
		}
	}
}