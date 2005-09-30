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
	using System.IO;
	using System.Reflection;
	using System.Collections;

	using Castle.ActiveRecord.Framework.Internal;

	using Castle.Components.Common.TemplateEngine;

	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Helpers;
	using Castle.MonoRail.ActiveRecordScaffold.Helpers;


	/// <summary>
	/// Renders a list of entities
	/// </summary>
	/// <remarks>
	/// Searchs for a template named <c>list{name}</c>
	/// </remarks>
	public class ListAction : AbstractScaffoldAction
	{
		public ListAction(Type modelType, ITemplateEngine templateEngine) : base(modelType, templateEngine)
		{
		}

		protected override void PerformActionProcess(Controller controller)
		{
			ObtainPKProperty();

			controller.PropertyBag.Add( "items", PaginationHelper.CreateCachedPagination(
				Model.Type.FullName, 10, new DataObtentionDelegate(PerformFindAll)) );
			controller.PropertyBag["model"] = Model;
			controller.PropertyBag["keyprop"] = keyProperty;
			controller.PropertyBag["properties"] = ObtainListableProperties(Model);

			controller.RenderView(controller.Name, "list" + Model.Type.Name);
		}

		private IList PerformFindAll()
		{
			return CommonOperationUtils.FindAll( Model.Type );
		}

		protected override string ComputeTemplateName(Controller controller)
		{
			return String.Format(@"{0}\list{1}", controller.Name, Model.Type.Name);
		}

		/// <summary>
		/// Called when the template was not found
		/// </summary>
		/// <param name="controller"></param>
		protected override void RenderStandardHtml(Controller controller)
		{
			SetUpHelpers(controller);

			StringWriter writer = new StringWriter();

			IDictionary context = new Hashtable();

			foreach(DictionaryEntry entry in controller.PropertyBag)
			{
				context.Add(entry.Key, entry.Value);
			}

#if DEBUG
			templateEngine.Process( context, "list.vm", writer );
#else
			templateEngine.Process( context, "Castle.MonoRail.ActiveRecordScaffold/Templates/list.vm", writer );
#endif

			controller.DirectRender( writer.GetStringBuilder().ToString() );

			// GenerateHtmlList(Model.Type.Name, Model, items, controller);
		}

		private IList ObtainListableProperties(ActiveRecordModel model)
		{
			ArrayList properties = new ArrayList();
	
			ObtainPKProperty();

			if (model.Parent != null)
			{
				properties.AddRange( ObtainListableProperties(model.Parent) );
			}
	
			foreach(PropertyModel propModel in model.Properties)
			{
				// TODO: Add ISet
				if (propModel.Property.PropertyType == typeof(IList)) continue;

				properties.Add(propModel.Property);
			}
	
			foreach(PropertyInfo prop in model.NotMappedProperties)
			{
				// TODO: Add ISet
				if (prop.PropertyType == typeof(IList)) continue;

				properties.Add(prop);
			}

			return properties;
		}

		private static void SetUpHelpers(Controller controller)
		{
			PresentationHelper presentationHelper = new PresentationHelper();
			presentationHelper.SetController(controller);
	
			PaginationHelper pageHelper = new PaginationHelper();
			pageHelper.SetController(controller);
	
			controller.PropertyBag["presentation"] = presentationHelper;
			controller.PropertyBag["pagination"] = pageHelper;
		}
	}
}
