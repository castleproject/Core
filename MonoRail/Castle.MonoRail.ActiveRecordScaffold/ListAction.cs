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
	using System.Text;
	using System.Reflection;
	using System.Collections;
	using Castle.MonoRail.ActiveRecordScaffold.Helpers;
	using Castle.MonoRail.Framework;

	using Castle.ActiveRecord.Framework.Internal;

	using Castle.Components.Common.TemplateEngine;
	using Castle.MonoRail.Framework.Helpers;


	/// <summary>
	/// Renders a list of entities
	/// </summary>
	/// <remarks>
	/// Searchs for a template named <c>list{name}</c>
	/// </remarks>
	public class ListAction : AbstractScaffoldAction
	{
		private readonly ITemplateEngine templateEngine;

		public ListAction(Type modelType, ITemplateEngine templateEngine) : base(modelType)
		{
			this.templateEngine = templateEngine;
		}

		protected override void PerformActionProcess(Controller controller)
		{
			ObtainPKProperty();

			PresentationHelper presentationHelper = new PresentationHelper();
			presentationHelper.SetController(controller);
			PaginationHelper pageHelper = new PaginationHelper();
			pageHelper.SetController(controller);

			controller.PropertyBag.Add( "items", PaginationHelper.CreateCachedPagination(
				Model.Type.FullName, 10, new DataObtentionDelegate(PerformFindAll)) );
			controller.PropertyBag["model"] = Model;
			controller.PropertyBag["keyprop"] = keyProperty;
			controller.PropertyBag["properties"] = ObtainListableProperties(Model);
			controller.PropertyBag["presentation"] = presentationHelper;
			controller.PropertyBag["pagination"] = pageHelper;

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

		private void GenerateHtmlList(String name, ActiveRecordModel model, object[] items, Controller controller)
		{
			StringBuilder sb = new StringBuilder();

			IList properties = ObtainListableProperties(model);

			StartTable(sb, properties);

			int index = 0;

			foreach(object item in items)
			{
				String styleClass = "scaffoldrow";

				if (index++ % 2 == 0) styleClass = "scaffoldaltrow";

				sb.AppendFormat( "\r\n\t<tr class=\"{0}\">", styleClass );

				object idVal = null;

				if (keyProperty != null)
				{
					idVal = keyProperty.GetGetMethod().Invoke( item, null );

					sb.AppendFormat( "\r\n\t\t<td align=\"center\">{0}</td>", idVal );
				}

				foreach(PropertyInfo prop in properties)
				{
					if (prop.PropertyType == typeof(String))
						sb.Append( "\r\n\t\t<td>" );
					else
					else
						sb.Append( "\r\n\t\t<td align=\"center\">" );

					object value = prop.GetValue(item, new object[0]);

					if (value != null)
					{
						sb.Append( value.ToString() );
					}
					
					sb.Append( "</td>" );
				}

				if (idVal != null)
				{
					sb.Append( "\r\n\t\t<td align=\"center\">" );
					sb.Append( helper.LinkTo( "Edit", controller.Name, "edit" + name, idVal ) );
					sb.Append( " | " );
					sb.Append( helper.LinkTo( "Delete", controller.Name, "confirm" + name, idVal ) );
					sb.Append( "</td>" );
				}
				else
				{
					sb.Append( "\r\n\t\t<td>&nbsp;</td>" );
				}

				sb.Append( "\r\n\t</tr>" );
			}

			EndTable(sb);

			sb.Append( "<p>\r\n" );
			sb.Append( helper.LinkTo( "New", controller.Name, "new" + name ) );
			sb.Append( "</p>\r\n" );

			controller.DirectRender( sb.ToString() );
		}

		private void StartTable(StringBuilder sb, IList properties)
		{
			sb.Append( "<table class=\"scaffoldtable\" cellpadding=\"2\" cellspacing=\"0\" border=\"0\" width=\"100%\">\r\n" );
			sb.Append( "\t<tr>" );

			// ID Column
			sb.AppendFormat( "\t\t<th>{0}</th>\r\n", "&nbsp;" );

			foreach(PropertyInfo prop in properties)
			{
				sb.AppendFormat( "\t\t<th>{0}</th>\r\n", prop.Name );
			}

			// Commands Columns
			sb.AppendFormat( "\t\t<th>{0}</th>\r\n", "&nbsp;" );

			sb.Append( "\t</tr>\r\n" );
		}

		private void EndTable(StringBuilder sb)
		{
			sb.Append( "\r\n</table>\r\n" );
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
				if (propModel.PropertyType == typeof(IList)) continue;

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
	}
}
