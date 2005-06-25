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

	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Helpers;

	using Castle.ActiveRecord.Framework.Internal;


	public abstract class AbstractScaffoldAction : IDynamicAction
	{
		protected readonly Type modelType;

		public AbstractScaffoldAction( Type modelType )
		{
			this.modelType = modelType;
		}

		public abstract void Execute(Controller controller);

		protected void GenerateHtml(string name, ActiveRecordModel model, object instance, Controller controller)
		{
			// In this case it's up to us to create the insertion form

			HtmlHelper helper = new HtmlHelper();
		
			StringBuilder sb = new StringBuilder();
	
			sb.AppendFormat( "<h3>New {0}</h3>\r\n", name );
			
			sb.Append( helper.Form( String.Format("create{0}.{1}", 
				name, controller.Context.UrlInfo.Extension) ) );
	
			foreach( PropertyModel prop in model.Properties )
			{
				// Skip non standard properties
				if (!prop.Property.CanWrite || !prop.Property.CanRead) continue;

				// Skip indexers
				if (prop.Property.GetGetMethod().GetParameters().Length != 0) continue;

				sb.Append( "<p>\r\n" );

				Type propType = prop.Property.PropertyType;
				String propName = prop.Property.Name;
				object value = prop.Property.GetGetMethod().Invoke( instance, null );

				sb.AppendFormat( helper.LabelFor( propName, propName + ':' ) );

				// TODO: Add proper validation scripts

				if (propType == typeof(String) && String.Compare("stringclob", prop.PropertyAtt.ColumnType, true) == 0)
				{
					sb.AppendFormat( helper.TextArea( propName, 30, 3, (String) value ) );
				}
				else if (propType == typeof(String) && prop.PropertyAtt.Length != 0)
				{
					sb.AppendFormat( helper.InputText( propName, (String) value, prop.PropertyAtt.Length, prop.PropertyAtt.Length ) );
				}
				else if (propType == typeof(String))
				{
					sb.AppendFormat( helper.InputText( propName, (String) value ) );
				}
				else if (propType == typeof(Int16) || propType == typeof(Int32) || propType == typeof(Int64))
				{
					sb.AppendFormat( helper.InputText( propName, value.ToString(), 10, 4 ) );
				}
				else if (propType == typeof(Single) || propType == typeof(Double))
				{
					sb.AppendFormat( helper.InputText( propName, value.ToString() ) );
				}
				else if (propType == typeof(DateTime))
				{
					sb.AppendFormat( helper.DateTime( propName, (DateTime) value ) );
				}
				else if (propType == typeof(bool))
				{
					// TODO
				}
				else if (propType == typeof(Enum))
				{
					// TODO
				}

				sb.Append( "</p>\r\n" );
			}
	
			sb.Append( helper.CreateSubmit( "Save" ) );
	
			sb.Append( helper.EndForm() );
	
			controller.DirectRender( sb.ToString() );
		}
	}
}
