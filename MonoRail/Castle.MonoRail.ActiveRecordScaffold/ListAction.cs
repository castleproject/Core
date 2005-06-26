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

		public override void Execute(Controller controller)
		{
			ActiveRecordModel model = GetARModel();

			SetDefaultLayout(controller);

			// TODO: Implement pagination and use the Sliced version of find all

			MethodInfo findAll = model.Type.GetMethod( "FindAll", BindingFlags.Static|BindingFlags.Public, null, new Type[0], null );

			object[] items = null;

			if (findAll != null)
			{
				items = (object[]) findAll.Invoke( null, null );
			}
			else
			{
				IList list = SupportingUtils.FindAll( model.Type );

				items = new object[list.Count];

				list.CopyTo(items, 0);
			}

			String name = model.Type.Name;
			String viewName = String.Format(@"{0}\list{1}", controller.Name, name);

			if (controller.HasTemplate(viewName))
			{
				// The programmer provided a custom view

				controller.PropertyBag.Add( "items", items );

				controller.RenderView(controller.Name, "list" + name);
			}
			else
			{
				GenerateListHtml(name, model, items, controller);
			}
		}

		private void GenerateListHtml(String name, ActiveRecordModel model, object[] items, Controller controller)
		{
			StringBuilder sb = new StringBuilder();

			IList properties = ObtainListableProperties(model);

			sb.AppendFormat( "<h3>{0}:</h3>\r\n", controller.Name );

			StartTable(sb, properties);

			foreach(object item in items)
			{
				sb.Append( "\r\n\t<tr>" );

				foreach(PropertyInfo prop in properties)
				{
					sb.Append( "\r\n\t\t<td>" );

					object value = prop.GetGetMethod().Invoke( item, null );

					if (value != null)
					{
						sb.Append( value.ToString() );
					}
					
					sb.Append( "</td>" );
				}

				sb.Append( "\r\n\t</tr>" );
			}

			EndTable(sb);

			sb.Append( "<p>\r\n" );
			sb.Append( helper.CreateSubmit( "New" ) );
			sb.Append( "</p>\r\n" );

			controller.DirectRender( sb.ToString() );
		}

		private void StartTable(StringBuilder sb, IList properties)
		{
			sb.Append( "<table cellpadding=0 cellspacing=0 border=\"1\" width=\"100%\">\r\n" );
			sb.Append( "<tr>" );

			foreach(PropertyInfo prop in properties)
			{
				sb.AppendFormat( "<th>{0}</th>\r\n", prop.Name );
			}

			sb.Append( "</tr>\r\n" );
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
				properties.Add(keyModel.Property);
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
