// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.ActiveRecordSupport.Scaffold
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

		/// <summary>
		/// Initializes a new instance of the <see cref="NewAction"/> class.
		/// </summary>
		/// <param name="modelType">Type of the model.</param>
		/// <param name="templateEngine">The template engine.</param>
		/// <param name="useModelName">Indicates that we should use the model name on urls</param>
		/// <param name="useDefaultLayout">Whether we should use our layout.</param>
		public NewAction(Type modelType, ITemplateEngine templateEngine, bool useModelName, bool useDefaultLayout) : 
			base(modelType, templateEngine, useModelName, useDefaultLayout)
		{
		}

		/// <summary>
		/// Computes the name of the template.
		/// </summary>
		/// <param name="controller">The controller.</param>
		/// <returns></returns>
		protected override string ComputeTemplateName(IControllerContext controller)
		{
			return String.Format(@"{0}\new{1}", controller.Name, Model.Type.Name);
		}

		/// <summary>
		/// Implementors should perform the action for the
		/// scaffolding, like new or create.
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerContext">The controller context.</param>
		protected override void PerformActionProcess(IEngineContext engineContext, IController controller, IControllerContext controllerContext)
		{
			base.PerformActionProcess(engineContext, controller, controllerContext);
			
			if (instance == null)
			{
				instance = Activator.CreateInstance(Model.Type);
			}

			string prefix = Model.Type.Name;

			controllerContext.PropertyBag["prefix"] = prefix;
			controllerContext.PropertyBag[prefix + "type"] = Model.Type;
			controllerContext.PropertyBag["instance"] = instance;
		}

		protected override void RenderStandardHtml(IEngineContext engineContext, IController controller, IControllerContext controllerContext)
		{
			SetUpHelpers(engineContext, controller, controllerContext);
			RenderFromTemplate("new.vm", engineContext, controller, controllerContext);
		}
	}
}
