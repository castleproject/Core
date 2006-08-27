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
	using System.Reflection;
	using System.Collections;

	using Castle.ActiveRecord.Framework.Internal;

	using Castle.Components.Common.TemplateEngine;

	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Helpers;

	using Iesi.Collections;


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
			controller.PropertyBag.Add( "items", 
				PaginationHelper.CreatePagination(PerformFindAll(), 10) );

			controller.PropertyBag["model"] = Model;
			controller.PropertyBag["keyprop"] = ObtainPKProperty();
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
			RenderFromTemplate("list.vm", controller);
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
				if (IsNotSupported(propModel.Property.PropertyType)) continue;

				properties.Add(propModel.Property);
			}
	
			foreach(PropertyInfo prop in model.NotMappedProperties)
			{
				if (IsNotSupported(prop.PropertyType)) continue;

				properties.Add(prop);
			}

			return properties;
		}

		private bool IsNotSupported(Type type)
		{
			bool isUnsupportedType = (type == typeof(IList) || 
				type == typeof(ISet) || 
				type == typeof(IDictionary));
#if DOTNET2
            if( !isUnsupportedType && type.IsGenericType ) 
			{
                isUnsupportedType = (typeof(System.Collections.Generic.ICollection<>).IsAssignableFrom(type.GetGenericTypeDefinition()) || typeof(System.Collections.Generic.IList<>).IsAssignableFrom(type.GetGenericTypeDefinition()) || typeof(System.Collections.Generic.IDictionary<,>).IsAssignableFrom(type.GetGenericTypeDefinition()));
            }
#endif
			return isUnsupportedType;
		}
	}
}
