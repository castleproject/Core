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
	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework.Internal;
	using Castle.Components.Binder;
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

		/// <summary>Constructs the data source for the binder</summary>
		protected TreeBuilder builder = new TreeBuilder();

		/// <summary>Binder that 'knows' ActiveRecord types</summary>
		protected ARDataBinder binder = new ARDataBinder();

		/// <summary>The model for the AR type we're dealing with</summary>
		private ActiveRecordModel model;

		/// <summary>Used to define if the model name should be present on the action name (urls)</summary>
		private readonly bool useModelName;

		/// <summary>Indicates that the controller has no layout, so we use ours</summary>
		private readonly bool useDefaultLayout;

		/// <summary>
		/// Initializes a new instance of the <see cref="AbstractScaffoldAction"/> class.
		/// </summary>
		/// <param name="modelType">Type of the model.</param>
		/// <param name="templateEngine">The template engine.</param>
		/// <param name="useModelName">Indicates that we should use the model name on urls</param>
		/// <param name="useDefaultLayout">Whether we should use our layout.</param>
		public AbstractScaffoldAction(Type modelType, ITemplateEngine templateEngine, 
		                              bool useModelName, bool useDefaultLayout)
		{
			this.modelType = modelType;
			this.templateEngine = templateEngine;
			this.useModelName = useModelName;
			this.useDefaultLayout = useDefaultLayout;

			// Configures the binder
			binder.AutoLoad = AutoLoadBehavior.OnlyNested;
		}

		/// <summary>
		/// Gets a value indicating whether the name of the model should
		/// be used on the url.
		/// </summary>
		/// <value><c>true</c> if yes, otherwise <c>false</c>.</value>
		public bool UseModelName
		{
			get { return useModelName; }
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
			// We make sure the code is always surrounded by a SessionScope.
			// If none is found, we create one
			
			SessionScope scope = null;
			
			if (SessionScope.Current == null)
			{
				scope = new SessionScope(FlushAction.Never);
			}

			try
			{
				model = GetARModel();

				PerformActionProcess(controller);

				String templateName = ComputeTemplateName(controller);

				if (controller.HasTemplate(templateName))
				{
					controller.RenderSharedView(templateName);
				}
				else
				{
					RenderStandardHtml(controller);
				}
			}
			finally
			{
				if (scope != null)
				{
					scope.Dispose();
				}
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
		/// Only invoked if the programmer havent provided
		/// a custom template for the current action. Implementors
		/// should create a basic html to present.
		/// </summary>
		/// <param name="controller"></param>
		protected abstract void RenderStandardHtml(Controller controller);

		/// <summary>
		/// Implementors should perform the action for the 
		/// scaffolding, like new or create.
		/// </summary>
		/// <param name="controller"></param>
		protected virtual void PerformActionProcess(Controller controller)
		{
			controller.PropertyBag["useModelName"] = useModelName;
			controller.PropertyBag["model"] = Model;
			controller.PropertyBag["keyprop"] = ObtainPKProperty();
		}

		/// <summary>
		/// Gets the current <see cref="ActiveRecordModel"/>
		/// </summary>
		protected ActiveRecordModel Model
		{
			get { return model; }
		}

		private ActiveRecordModel GetARModel()
		{
			ActiveRecordModel foundModel = ActiveRecordModel.GetModel(modelType);
	
			if (foundModel == null)
			{
				throw new ScaffoldException("Specified type is not an ActiveRecord type or the ActiveRecord " + 
					"framework was not started properly. Did you forget to invoke ActiveRecordStarter.Initialize() ?");
			}

			return foundModel;
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

			if (useDefaultLayout)
			{
				StringWriter layoutwriter = new StringWriter();
				
				context.Add("childContent", writer.GetStringBuilder().ToString());
				
#if USE_LOCAL_TEMPLATES
				templateEngine.Process(context, "layout.vm", layoutwriter);
#else
				templateEngine.Process(context, "Castle.MonoRail.ActiveRecordScaffold/Templates/layout.vm", layoutwriter);
#endif
				
				writer = layoutwriter;
			}

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
		
		protected static void AssertIsPost(Controller controller)
		{
			String method = controller.Context.UnderlyingContext.Request.HttpMethod;
			
			if (method != "POST")
			{
				throw new Exception("This action cannot be accessed using the verb " + method);
			}
		}
	}
}
