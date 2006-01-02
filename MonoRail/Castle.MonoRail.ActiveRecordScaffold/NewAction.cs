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

	using Castle.MonoRail.Framework;

	using Castle.Components.Common.TemplateEngine;


	/// <summary>
	/// Renders an inclusion form
	/// </summary>
	/// <remarks>
	/// Searchs for a template named <c>new{name}</c>
	/// </remarks>
	public class NewAction : AbstractScaffoldAction
	{
		protected object instance;

		public NewAction(Type modelType, ITemplateEngine templateEngine) : base(modelType, templateEngine)
		{
		}

		protected override string ComputeTemplateName(Controller controller)
		{
			return String.Format(@"{0}\new{1}", controller.Name, Model.Type.Name);
		}

		protected override void PerformActionProcess(Controller controller)
		{
			if (instance == null)
			{
				instance = Activator.CreateInstance( Model.Type );
			}

			controller.PropertyBag["armodel"] = Model;
			controller.PropertyBag["instance"] = instance;
		}

		protected override void RenderStandardHtml(Controller controller)
		{
			SetUpHelpers(controller);
			RenderFromTemplate("new.vm", controller);
		}
	}
}
