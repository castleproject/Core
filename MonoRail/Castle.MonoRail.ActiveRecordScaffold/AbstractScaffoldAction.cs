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
	using System.IO;
	using System.Reflection;
	using System.Collections;

	using Castle.ActiveRecord.Framework.Internal;

	using Castle.Components.Common.TemplateEngine;

	using Castle.MonoRail.ActiveRecordScaffold.Helpers;
	using Castle.MonoRail.ActiveRecordSupport;
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Helpers;

	/// <summary>
	/// Base abstract class for actions that relate to 
	/// Scaffolding support. Provide the basic flow process
	/// </summary>
	public abstract class AbstractScaffoldAction : IDynamicAction
	{
		/// <summary>Holds the AR type</summary>
		protected readonly Type modelType;
		
		/// <summary>Reference to the template engine instance</summary>
		protected readonly ITemplateEngine templateEngine;
		
		/// <summary>A map of PropertyInfo to validation failures</summary>
		protected IDictionary prop2Validation = new Hashtable();
		
		/// <summary>A list of errors that happened during this process</summary>
		protected ArrayList errors = new ArrayList();

		/// <summary>The model for the AR type we're dealing with</summary>
		private ActiveRecordModel model;

		public AbstractScaffoldAction( Type modelType, ITemplateEngine templateEngine )
		{
			this.modelType = modelType;
			this.templateEngine = templateEngine;
		}

		/// <summary>
		/// Executes the basic flow which is
		/// <list type="number">
		/// <item><description>Resolve the <see cref="ActiveRecordModel"/></description></item>
		/// <item><description>Resolve the layout (if not is associated with the controller, defaults to "scaffold")</description></item>
		/// <item><description>Invokes <see cref="PerformActionProcess"/> which should perform the correct process for this action</description></item>
		/// <item><description>Resolves the template name that the developer might provide by using <see cref="ComputeTemplateName"/></description></item>
		/// <item><description>If the template exists, renders it. Otherwise invokes <see cref="RenderStandardHtml"/></description></item>
		/// </list>
		/// </summary>
		/// <param name="controller"></param>
		public void Execute(Controller controller)
		{
			model = GetARModel();

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

		/// <summary>
		/// Implementors should return the template name 
		/// for the current action.
		/// </summary>
		/// <param name="controller"></param>
		/// <returns></returns>
		protected abstract string ComputeTemplateName(Controller controller);

		/// <summary>
		/// Implementors should perform the action for the 
		/// scaffolding, like new or create.
		/// </summary>
		/// <param name="controller"></param>
		protected abstract void PerformActionProcess(Controller controller);

		/// <summary>
		/// Only invoked if the programmer havent provided
		/// a custom template for the current action. Implementors
		/// should create a basic html to present.
		/// </summary>
		/// <param name="controller"></param>
		protected abstract void RenderStandardHtml(Controller controller);

		/// <summary>
		/// Gets the current <see cref="ActiveRecordModel"/>
		/// </summary>
		protected ActiveRecordModel Model
		{
			get { return model; }
		}

		private ActiveRecordModel GetARModel()
		{
			ActiveRecordModel model = ActiveRecordModel.GetModel( modelType );
	
			if (model == null)
			{
				throw new ScaffoldException("Specified type isn't an ActiveRecord type or the ActiveRecord " + 
					"framework wasn't started properly. Did you forget about the Initialize method?");
			}

			return model;
		}

		/// <summary>
		/// Checks whether the controller has 
		/// a layout set up, if it doesn't sets <c>scaffold</c>
		/// as the layout (which must exists on the view tree)
		/// </summary>
		/// <param name="controller"></param>
		protected void SetDefaultLayout(Controller controller)
		{
			if (controller.LayoutName == null)
			{
				controller.LayoutName = "scaffold";
			}
		}

		/// <summary>
		/// Gets the property that represents the Primary key 
		/// for the current <see cref="ActiveRecordModel"/>
		/// </summary>
		/// <returns></returns>
		protected PropertyInfo ObtainPKProperty()
		{
			PrimaryKeyModel keyModel = ARCommonUtils.ObtainPKProperty(model);
			
			if (keyModel != null)
			{
				return keyModel.Property;
			}

			return null;
		}

		protected void RenderFromTemplate(String templateName, Controller controller)
		{
			StringWriter writer = new StringWriter();

			IDictionary context = new Hashtable();

			context.Add("flash", controller.Context.Flash);

			foreach(DictionaryEntry entry in controller.PropertyBag)
			{
				context.Add(entry.Key, entry.Value);
			}

#if USE_LOCAL_TEMPLATES
			templateEngine.Process( context, templateName, writer );
#else
			templateEngine.Process( context, "Castle.MonoRail.ActiveRecordScaffold/Templates/" + templateName, writer );
#endif

			controller.DirectRender( writer.GetStringBuilder().ToString() );		
		}

		protected static void SetUpHelpers(Controller controller)
		{
			ARFormHelper htmlHelper = new ARFormHelper();
			htmlHelper.SetController(controller);
	
			ValidationHelper validationHelper = new ValidationHelper();
			validationHelper.SetController(controller);
	
			PresentationHelper presentationHelper = new PresentationHelper();
			presentationHelper.SetController(controller);
	
            PaginationHelper paginationHelper = new PaginationHelper();
            paginationHelper.SetController(controller);

			controller.PropertyBag["HtmlHelper"] = htmlHelper;
			controller.PropertyBag["ValidationHelper"] = validationHelper;
			controller.PropertyBag["PresentationHelper"] = presentationHelper;
            controller.PropertyBag["PaginationHelper"] = paginationHelper;
		}
	}
}
