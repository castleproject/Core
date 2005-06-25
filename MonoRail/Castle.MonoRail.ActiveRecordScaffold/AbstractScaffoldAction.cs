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

	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Helpers;

	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework.Internal;


	public abstract class AbstractScaffoldAction : IDynamicAction
	{
		protected readonly Type modelType;
		protected readonly HtmlHelper helper = new HtmlHelper();
		protected readonly AjaxHelper ajax = new AjaxHelper();
		protected readonly Effects2Helper effects = new Effects2Helper();

		public AbstractScaffoldAction( Type modelType )
		{
			this.modelType = modelType;
		}

		public abstract void Execute(Controller controller);

		protected void GenerateHtml(string name, ActiveRecordModel model, object instance, Controller controller)
		{
			// In this case it's up to us to create the insertion form
		
			StringBuilder sb = new StringBuilder();

			sb.Append( ajax.GetJavascriptFunctions() );
			sb.Append( effects.GetJavascriptFunctions() );
				
			sb.Append( helper.Form( String.Format("create{0}.{1}", 
				name, controller.Context.UrlInfo.Extension) ) );

			sb.Append( helper.FieldSet( "New " + name + ':' ) );

			RecursiveGenerateHtml( model, model.Type, instance, controller, sb );
		
			sb.Append( helper.EndFieldSet() );

			sb.Append( "<p>\r\n" );
			sb.Append( helper.CreateSubmit( "Save" ) );
			sb.Append( "  " );
			sb.Append( helper.CreateSubmit( "Cancel" ) );
			sb.Append( "</p>\r\n" );
	
			sb.Append( helper.EndForm() );
	
			controller.DirectRender( sb.ToString() );
		}

		protected void RecursiveGenerateHtml(ActiveRecordModel model, Type type, object instance, Controller controller, StringBuilder sb)
		{
			if ( type.BaseType != typeof(object) && 
				type.BaseType != typeof(ActiveRecordBase) && 
				type.BaseType != typeof(ActiveRecordValidationBase) )
			{
				RecursiveGenerateHtml( ActiveRecordBase._GetModel( type.BaseType ), type.BaseType, instance, controller, sb );
			}

			foreach( PropertyModel prop in model.Properties )
			{
				// Skip non standard properties
				if (!prop.Property.CanWrite || !prop.Property.CanRead) continue;

				// Skip indexers
				if (prop.Property.GetGetMethod().GetParameters().Length != 0) continue;

				sb.Append( "<p>\r\n" );

				Type propType = prop.Property.PropertyType;
				String propName = prop.Property.Name;
				object value = null;
				
				if (instance != null)
				{
					value = prop.Property.GetGetMethod().Invoke( instance, null );
				}

				sb.AppendFormat( helper.LabelFor( propName, propName + ':' ) );

				RenderHtmlControl(propType, prop, sb, propName, value);

				sb.Append( "</p>\r\n" );
			}

			foreach( PropertyInfo prop in model.NotMappedProperties )
			{
				// Skip non standard properties
				if (!prop.CanWrite || !prop.CanRead) continue;

				// Skip indexers
				if (prop.GetGetMethod().GetParameters().Length != 0) continue;

				sb.Append( "<p>\r\n" );

				Type propType = prop.PropertyType;
				String propName = prop.Name;
				object value = null;
				
				if (instance != null)
				{
					value = prop.GetGetMethod().Invoke( instance, null );
				}

				sb.AppendFormat( helper.LabelFor( propName, propName + ':' ) );

				RenderHtmlControl(propType, null, sb, propName, value);

				sb.Append( "</p>\r\n" );
			}

			foreach( NestedModel nested in model.Components )
			{
				object nestedInstance = nested.Property.GetGetMethod().Invoke( instance, null );

				sb.Append( helper.FieldSet( nested.Property.Name + ':' ) );
				
				RecursiveGenerateHtml( nested.Model, nested.Model.Type, nestedInstance, controller, sb );
		
				sb.Append( helper.EndFieldSet() );
			}
		}

		protected ActiveRecordModel GetARModel()
		{
			ActiveRecordModel model = ActiveRecordBase._GetModel( modelType );
	
			if (model == null)
			{
				throw new ScaffoldException("Specified type isn't an ActiveRecord type or the ActiveRecord " + 
					"framework wasn't started properly. Did you forget about the Initialize method?");
			}

			return model;
		}

		protected void SetDefaultLayout(Controller controller)
		{
			if (controller.LayoutName == null)
			{
				controller.LayoutName = "Scaffold";
			}
		}

		private void RenderHtmlControl(Type propType, PropertyModel prop, StringBuilder sb, string propName, object value)
		{
			// TODO: Add proper validation scripts
	
			if (propType == typeof(String) && prop != null && String.Compare("stringclob", prop.PropertyAtt.ColumnType, true) == 0)
			{
				sb.AppendFormat( helper.TextArea( propName, 30, 3, (String) value ) );
			}
			else if (propType == typeof(String) && prop != null && prop.PropertyAtt.Length != 0)
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
		}
	}
}
