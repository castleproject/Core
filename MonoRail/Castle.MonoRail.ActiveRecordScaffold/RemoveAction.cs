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
	using Castle.MonoRail.Framework;
	using Castle.Components.Common.TemplateEngine;

	/// <summary>
	/// Removes the ActiveRecord instance
	/// </summary>
	/// <remarks>
	/// Searchs for a template named <c>{name}removed</c>
	/// </remarks>
	public class RemoveAction : AbstractScaffoldAction
	{
		public RemoveAction(Type modelType, ITemplateEngine templateEngine) : base(modelType, templateEngine)
		{
		}

		protected override string ComputeTemplateName(Controller controller)
		{
			return String.Format(@"{0}\{1}removed", controller.Name, Model.Type.Name);
		}

		protected override void PerformActionProcess(Controller controller)
		{
			object idVal = CommonOperationUtils.ReadPkFromParams(controller, ObtainPKProperty());

			controller.PropertyBag["armodel"] = Model;
			controller.PropertyBag["id"] = idVal;

			try
			{
				object instance = ActiveRecordMediator.FindByPrimaryKey(Model.Type, idVal, true);

				controller.PropertyBag["instance"] = instance;

				ActiveRecordMediator.Delete(instance);
			}
			catch(Exception ex)
			{
				controller.PropertyBag["exception"] = ex;
			}
		}

		protected override void RenderStandardHtml(Controller controller)
		{
			SetUpHelpers(controller);
			RenderFromTemplate("remove.vm", controller);
		}
	}
}