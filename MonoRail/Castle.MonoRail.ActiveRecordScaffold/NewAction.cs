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

	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework.Internal;

	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Helpers;

	using Iesi.Collections;


	/// <summary>
	/// Renders an inclusion form
	/// </summary>
	/// <remarks>
	/// Searchs for a template named <c>new{name}</c>
	/// </remarks>
	public class NewAction : AbstractScaffoldAction
	{
		protected object instance;
		protected DataBinder binder;
		protected ArrayList errors = new ArrayList();


		public NewAction( Type modelType ) : base(modelType)
		{
		}

		protected override string ComputeTemplateName(Controller controller)
		{
			return String.Format(@"{0}\new{1}", controller.Name, Model.Type.Name);
		}

		protected override void PerformActionProcess(Controller controller)
		{
			controller.PropertyBag["armodel"] = Model;
		}

		protected override void RenderStandardHtml(Controller controller)
		{
			if (instance == null)
			{
				instance = Activator.CreateInstance( Model.Type );
			}

			GenerateHtmlForm(Model.Type.Name, Model, instance, controller, "New");
		}

		/// <summary>
		/// Constructs a html form
		/// </summary>
		protected void GenerateHtmlForm(String name, ActiveRecordModel model, object instance, Controller controller, String fieldSetTitle)
		{
			StringBuilder sb = new StringBuilder();

			CreateForm(sb, name, controller);

			ArrayList errors = (ArrayList) controller.Context.Flash["errors"];

			if (errors != null)
			{
				sb.Append( HtmlHelper.BuildUnorderedList( (String[]) errors.ToArray( typeof(String) ), 
					"errorList", "errorMessage" ) );
			}

			sb.Append( helper.FieldSet( fieldSetTitle + " " + name + ':' ) );

			RecursiveGenerateHtmlForm( name, model, model.Type, instance, controller, sb );
		
			sb.Append( helper.EndFieldSet() );

			sb.Append( "<p>\r\n" );
			sb.Append( helper.CreateSubmit( "Save" ) );
			sb.Append( "  " );
			sb.Append( helper.LinkTo( "Cancel", controller.Name, "list" + name ) );
			sb.Append( "</p>\r\n" );
	
			AddHiddenFields( sb );

			sb.Append( helper.EndForm() );
	
			controller.DirectRender( sb.ToString() );
		}

		protected virtual void AddHiddenFields(StringBuilder sb)
		{
		}

		protected virtual void CreateForm(StringBuilder sb, String name, Controller controller)
		{
			sb.Append( helper.Form( String.Format("create{0}.{1}", 
			                                      name, controller.Context.UrlInfo.Extension) ) );
		}

		protected void RecursiveGenerateHtmlForm(String name, ActiveRecordModel model, Type type, object instance, Controller controller, StringBuilder sb)
		{
			if ( type.BaseType != typeof(object) && 
				type.BaseType != typeof(ActiveRecordBase) && 
				type.BaseType != typeof(ActiveRecordValidationBase) )
			{
				RecursiveGenerateHtmlForm( name, ActiveRecordBase._GetModel( type.BaseType ), type.BaseType, instance, controller, sb );
			}

			RenderProperties(model, sb, instance, name);

			RenderBelongsToProperties(model, sb, instance);

			RenderHasManyProperties(model, sb, instance);

			RenderNestedComponents(model, instance, sb, controller);
		}

		protected void CreateInstanceFromFormData(Controller controller)
		{
			binder = new DataBinder(controller.Context);
	
			instance = binder.BindObject( Model.Type );
	
			SaveManyMappings(controller);
		}

		protected void CreateInstanceFromFormData(object instance, Controller controller)
		{
			binder = new DataBinder(controller.Context);
	
			instance = binder.BindObjectInstance( instance, String.Empty );
	
			SaveManyMappings(controller);
		}

		protected void SaveManyMappings(Controller controller)
		{
			foreach(HasManyModel hasManyModel in Model.HasMany)
			{
				if (hasManyModel.HasManyAtt.Inverse) continue;
				if (hasManyModel.HasManyAtt.RelationType != RelationType.Bag && 
					hasManyModel.HasManyAtt.RelationType != RelationType.Set) continue;

				ActiveRecordModel otherModel = ActiveRecordBase._GetModel(hasManyModel.HasManyAtt.MapType);

				PrimaryKeyModel keyModel = ObtainPKProperty(otherModel);

				if (otherModel == null || keyModel == null)
				{
					continue; // Impossible to save
				}

				CreateMappedInstances(hasManyModel.Property, keyModel, controller, otherModel);
			}

			foreach(HasAndBelongsToManyModel hasManyModel in Model.HasAndBelongsToMany)
			{
				if (hasManyModel.HasManyAtt.Inverse) continue;
				if (hasManyModel.HasManyAtt.RelationType != RelationType.Bag && 
					hasManyModel.HasManyAtt.RelationType != RelationType.Set) continue;

				ActiveRecordModel otherModel = ActiveRecordBase._GetModel(hasManyModel.HasManyAtt.MapType);

				PrimaryKeyModel keyModel = ObtainPKProperty(otherModel);

				if (otherModel == null || keyModel == null)
				{
					continue; // Impossible to save
				}

				CreateMappedInstances(hasManyModel.Property, keyModel, controller, otherModel);
			}
		}

		private void CreateMappedInstances(PropertyInfo prop, PrimaryKeyModel keyModel, Controller controller, ActiveRecordModel otherModel)
		{
			object container = InitializeRelationPropertyIfNull(prop);
	
			// TODO: Support any kind of key
	
			String paramName = String.Format( "{0}.{1}", prop.Name, keyModel.Property.Name );
	
			String[] values = controller.Context.Params.GetValues( paramName );
	
			int[] ids = (int[]) DataBinder.Convert( typeof(int[]), values, paramName, null, controller.Context );
	
			if (ids != null)
			{
				foreach(int id in ids)
				{
					object item = Activator.CreateInstance( otherModel.Type );

					keyModel.Property.SetValue( item, id, Empty );

					AddToContainer(container, item);
				}
			}
		}

		private object InitializeRelationPropertyIfNull(PropertyInfo property)
		{
			return InitializeRelationPropertyIfNull(instance, property);
		}

		private static object InitializeRelationPropertyIfNull( object instance, PropertyInfo property)
		{
			object container = property.GetValue( instance, Empty );

			if (container == null)
			{
				if (property.PropertyType == typeof(IList))
				{
					container = new ArrayList();
				}
				else if (property.PropertyType == typeof(ISet))
				{
					container = new HashedSet();
				}

				property.SetValue( instance, container, Empty );
			}

			return container;
		}

		private void AddToContainer(object container, object item)
		{
			if (container is IList)
			{
				(container as IList).Add( item );
			}
			else if (container is ISet)
			{
				(container as ISet).Add( item );
			}
		}

		private void RenderNestedComponents(ActiveRecordModel model, object instance, StringBuilder sb, Controller controller)
		{
			foreach( NestedModel nested in model.Components )
			{
				object nestedInstance = nested.Property.GetGetMethod().Invoke( instance, null );

				sb.Append( helper.FieldSet( nested.Property.Name + ':' ) );
				
				RecursiveGenerateHtmlForm( nested.Property.Name, nested.Model, nested.Model.Type, nestedInstance, controller, sb );
		
				sb.Append( helper.EndFieldSet() );
			}
		}

		private void RenderHasManyProperties(ActiveRecordModel model, StringBuilder sb, object instance)
		{
			foreach(HasManyModel hasManyModel in model.HasMany)
			{
				if (hasManyModel.HasManyAtt.Inverse) continue;

				PropertyInfo prop = hasManyModel.Property;

				ActiveRecordModel otherModel = ActiveRecordBase._GetModel(hasManyModel.HasManyAtt.MapType);

				PrimaryKeyModel keyModel = ObtainPKProperty(otherModel);

				if (otherModel == null || keyModel == null)
				{
					continue; // Impossible to output such model
				}

				RenderMultipleSelect(instance, prop, keyModel, otherModel, sb, hasManyModel.HasManyAtt.Where);
			}
	
			foreach(HasAndBelongsToManyModel hasManyModel in model.HasAndBelongsToMany)
			{
				if (hasManyModel.HasManyAtt.Inverse) continue;

				PropertyInfo prop = hasManyModel.Property;

				ActiveRecordModel otherModel = ActiveRecordBase._GetModel(hasManyModel.HasManyAtt.MapType);

				PrimaryKeyModel keyModel = ObtainPKProperty(otherModel);

				if (otherModel == null || keyModel == null)
				{
					continue; // Impossible to output such model
				}

				RenderMultipleSelect(instance, prop, keyModel, otherModel, sb, hasManyModel.HasManyAtt.Where);
			}
		}

		private void RenderMultipleSelect(object instance, PropertyInfo prop, PrimaryKeyModel keyModel, ActiveRecordModel otherModel, StringBuilder sb, String customWhere)
		{
			object container = InitializeRelationPropertyIfNull(instance, prop);
	
			object value = null;
	
			if (container != null)
			{
				value = CreateArrayFromExistingIds(keyModel, container as ICollection);
			}
	
			object[] items = CommonOperationUtils.FindAll( otherModel.Type, customWhere );
	
			sb.Append( "<p>\r\n" );
	
			String propName = String.Format( "{0}.{1}", prop.Name, keyModel.Property.Name );
	
			sb.Append( helper.LabelFor( propName, prop.Name + ':', new DictHelper().CreateDict( "style=vertical-align: top;" ) ) );
	
			sb.Append( helper.Select( propName, new DictHelper().CreateDict("size=5", "multiple") ) );
	
			sb.Append( helper.CreateOptionsFromArray( items, null, keyModel.Property.Name, value ) );
	
			sb.Append( helper.EndSelect() );
	
			sb.Append( "</p>\r\n" );
		}

		private void RenderBelongsToProperties(ActiveRecordModel model, StringBuilder sb, object instance)
		{
			foreach(BelongsToModel belongsToModel in model.BelongsTo)
			{
				PropertyInfo prop = belongsToModel.Property;

				ActiveRecordModel otherModel = ActiveRecordBase._GetModel(belongsToModel.BelongsToAtt.Type);

				PrimaryKeyModel keyModel = ObtainPKProperty(otherModel);

				if (otherModel == null || keyModel == null)
				{
					continue; // Impossible to output such model
				}

				object[] items = CommonOperationUtils.FindAll( otherModel.Type );

				sb.Append( "<p>\r\n" );

				String propName = String.Format( "{0}.{1}", prop.Name, keyModel.Property.Name );
				object value = null;
				
				if (instance != null)
				{
					object temp = prop.GetValue( instance, Empty );
					
					if (temp != null)
					{
						value = keyModel.Property.GetValue( temp, Empty );
					}
				}

				sb.Append( helper.LabelFor( propName, prop.Name + ':' ) );

				sb.Append( helper.Select( propName ) );

				sb.Append( helper.CreateOptionsFromArray( items, null, keyModel.Property.Name, value ) );

				sb.Append( helper.EndSelect() );

				sb.Append( "</p>\r\n" );
			}
		}

		private void RenderProperties(ActiveRecordModel model, StringBuilder sb, object instance, string name)
		{
			foreach( PropertyModel prop in model.Properties )
			{
				// Skip non standard properties
				if (!prop.Property.CanWrite || !prop.Property.CanRead) continue;

				// Skip indexers
				if (prop.Property.GetIndexParameters().Length != 0) continue;

				sb.Append( "<p>\r\n" );

				Type propType = prop.Property.PropertyType;
				String propName = prop.Property.Name;
				object value = null;
				
				if (instance != null)
				{
					value = prop.Property.GetGetMethod().Invoke( instance, null );
				}

				sb.AppendFormat( helper.LabelFor( propName, propName + ':' ) );

				if (!model.IsNestedType)
				{
					RenderHtmlControl(propType, prop, sb, propName, value);
				}
				else
				{
					RenderHtmlControl(propType, prop, sb, name + "." + propName, value);
				}

				sb.Append( "</p>\r\n" );
			}
	
			foreach( PropertyInfo prop in model.NotMappedProperties )
			{
				// Skip non standard properties
				if (!prop.CanWrite || !prop.CanRead) continue;

				// Skip indexers
				if (prop.GetIndexParameters().Length != 0) continue;

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
		}

		private Array CreateArrayFromExistingIds( PrimaryKeyModel keyModel, ICollection container )
		{
			if (container == null || container.Count == 0) return null;

			Array array = Array.CreateInstance( keyModel.Property.PropertyType, container.Count );
			
			int index = 0;

			foreach(object item in container)
			{
				object val = keyModel.Property.GetValue( item, Empty );

				array.SetValue( val, index++ );
			}

			return array;
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
