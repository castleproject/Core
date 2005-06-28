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
	using System.Text;
	using System.Reflection;
	using System.Collections;

	using Castle.MonoRail.Framework;

	using Castle.ActiveRecord.Framework;
	using Castle.ActiveRecord.Framework.Internal;


	public class ListAction : AbstractScaffoldAction
	{
		public ListAction(Type modelType) : base(modelType)
		{
		}

		protected override void PerformActionProcess(Controller controller)
		{
			MethodInfo findAll = Model.Type.GetMethod( "FindAll", BindingFlags.Static|BindingFlags.Public, null, new Type[0], null );

			object[] items = null;

			if (findAll != null)
			{
				items = (object[]) findAll.Invoke( null, null );
			}
			else
			{
				IList list = SupportingUtils.FindAll( Model.Type );

				items = new object[list.Count];

				list.CopyTo(items, 0);
			}

			controller.PropertyBag["items"] = items;

			controller.RenderView(controller.Name, "list" + Model.Type.Name);
		}

		protected override string ComputeTemplateName(Controller controller)
		{
			return String.Format(@"{0}\list{1}", controller.Name, Model.Type.Name);
		}

		protected override void RenderStandardHtml(Controller controller)
		{
			object[] items = (object[]) controller.PropertyBag["items"];

			GenerateHtmlList(Model.Type.Name, Model, items, controller);
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
						sb.Append( "\r\n\t\t<td align=\"center\">" );

					object value = prop.GetGetMethod().Invoke( item, null );

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
					sb.Append( helper.LinkTo( "Delete", controller.Name, "remove" + name, idVal ) );
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
			IList properties = new ArrayList();
	
			foreach(PrimaryKeyModel keyModel in model.Ids)
			{
				keyProperty = keyModel.Property;
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
	}
}
