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

namespace Castle.MonoRail.ActiveRecordScaffold.Helpers
{
	using System;
	using System.Collections;
	using System.Text;
	using System.Reflection;

	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework;
	using Castle.ActiveRecord.Framework.Internal;
	using Castle.ActiveRecord.Framework.Validators;

	using Castle.MonoRail.Framework.Helpers;

	using Iesi.Collections;


	public class ARFormHelper : HtmlHelper
	{
		private static readonly object[] Empty = new object[0];

		private StringBuilder stringBuilder = new StringBuilder(1024);
		
		private IDictionary model2nestedInstance = new Hashtable();

		public ICollection GetModelHierarchy(ActiveRecordModel model, object instance)
		{
			ArrayList list = new ArrayList();

			ActiveRecordModel hierarchy = model;

			while(hierarchy != null)
			{
				list.Add(hierarchy);

				hierarchy = ActiveRecordModel.GetModel( hierarchy.Type.BaseType );
			}

			hierarchy = model;

			while(hierarchy != null)
			{
				foreach(NestedModel nested in hierarchy.Components)
				{
					object nestedInstance = nested.Property.GetValue( instance, null );

					if (nestedInstance == null)
					{
						nestedInstance = CreationUtil.Create( nested.Property.PropertyType );
					}

					if (nestedInstance != null)
					{
						model2nestedInstance[nested.Model] = nestedInstance;
					}

					list.Add( nested.Model );
				}

				hierarchy = ActiveRecordModel.GetModel( hierarchy.Type.BaseType );
			}

			return list;
		}

		#region CanHandle methods

		public bool CanHandle(FieldModel field)
		{
			return CanHandleType(field.Field.FieldType);
		}

		public bool CanHandle(PropertyModel propModel)
		{
			return CanHandleType(propModel.Property.PropertyType);
		}

		public bool CanHandle(PropertyInfo propInfo)
		{
			return CanHandleType(propInfo.PropertyType);
		}

		public bool CanHandle(BelongsToModel model)
		{
			return CheckModelAndKeyAreAccessible(model.BelongsToAtt.Type);
		}

		public bool CanHandle(HasManyModel model)
		{
			if (!model.HasManyAtt.Inverse)
			{
				return CheckModelAndKeyAreAccessible(model.HasManyAtt.MapType);
			}
			return false;
		}

		public bool CanHandle(HasAndBelongsToManyModel model)
		{
			if (!model.HasManyAtt.Inverse)
			{
				return CheckModelAndKeyAreAccessible(model.HasManyAtt.MapType);
			}
			return false;
		}

		private bool CheckModelAndKeyAreAccessible(Type type)
		{
			ActiveRecordModel otherModel = ActiveRecordModel.GetModel(type);

			PrimaryKeyModel keyModel = ObtainPKProperty(otherModel);

			if (otherModel == null || keyModel == null)
			{
				return false;
			}

			return true;
		}

		private PrimaryKeyModel ObtainPKProperty(ActiveRecordModel model)
		{
			if (model == null) return null;

			ActiveRecordModel curModel = model;

			while (curModel != null)
			{
				foreach(PrimaryKeyModel keyModel in curModel.Ids)
				{
					return keyModel;
				}

				curModel = curModel.Parent;
			}

			return null;
		}

		private bool CanHandleType(Type type)
		{
			return (type == typeof(String)
				 || type == typeof(Int16)
				 || type == typeof(Int32)
				 || type == typeof(Int64)
				 || type == typeof(Decimal)
				 || type == typeof(Single)
				 || type == typeof(Double)
				 || type == typeof(Byte)
				 || type == typeof(SByte)
				 || type == typeof(bool)
				 || type == typeof(Enum)
				 || type == typeof(DateTime));
		}

		#endregion

		#region CreateControl methods

		public String CreateControl(ActiveRecordModel model, FieldModel fieldModel, object instance)
		{
			stringBuilder.Length = 0;

			FieldInfo fieldInfo = fieldModel.Field;

			object value = null;
				
			if (instance != null)
			{
				if (model.IsNestedType)
				{
					instance = model2nestedInstance[model];
				}

				if (instance != null) value = fieldInfo.GetValue(instance);
			}

			String propName = null;

			if (model.IsNestedType)
			{
				propName = String.Format("{0}.{1}", model.Type.Name, fieldInfo.Name);
			}
			else
			{
				propName = fieldInfo.Name;
			}

			if (fieldInfo.FieldType == typeof(DateTime))
			{
				stringBuilder.Append( LabelFor( propName + "day", fieldInfo.Name + ": &nbsp;" ) );
			}
			else
			{
				stringBuilder.Append( LabelFor( propName, fieldInfo.Name + ": &nbsp;" ) );
			}

			FieldAttribute propAtt = fieldModel.FieldAtt;

			RenderAppropriateControl(model, fieldInfo.FieldType, propName, null, value, 
				propAtt.Unique, propAtt.NotNull, propAtt.ColumnType, propAtt.Length);

			return stringBuilder.ToString();
		}

		public String CreateControl(ActiveRecordModel model, PropertyModel propertyModel, object instance)
		{
			stringBuilder.Length = 0;

			PropertyInfo prop = propertyModel.Property;

			// Skip non standard properties
			if (!prop.CanWrite || !prop.CanRead) return String.Empty;

			// Skip indexers
			if (prop.GetIndexParameters().Length != 0) return String.Empty;

			object value = null;
				
			if (instance != null)
			{
				if (model.IsNestedType)
				{
					instance = model2nestedInstance[model];
				}

				if (instance != null) value = prop.GetValue(instance, null);
			}

			String propName = null;

			if (model.IsNestedType)
			{
				propName = String.Format("{0}.{1}", model.Type.Name, prop.Name);
			}
			else
			{
				propName = prop.Name;
			}
			
			if (prop.PropertyType == typeof(DateTime))
			{
				stringBuilder.Append( LabelFor( propName + "day", prop.Name + ": &nbsp;" ) );
			}
			else
			{
				stringBuilder.Append( LabelFor( propName, prop.Name + ": &nbsp;" ) );
			}

			PropertyAttribute propAtt = propertyModel.PropertyAtt;
			
			RenderAppropriateControl(model, prop.PropertyType, propName, prop, value, 
				propAtt.Unique, propAtt.NotNull, propAtt.ColumnType, propAtt.Length);

			return stringBuilder.ToString();
		}

		public String CreateControl(ActiveRecordModel model, PropertyInfo prop, object instance)
		{
			stringBuilder.Length = 0;

			// Skip non standard properties
			if (!prop.CanWrite || !prop.CanRead) return String.Empty;

			// Skip indexers
			if (prop.GetIndexParameters().Length != 0) return String.Empty;

			object value = null;
				
			if (instance != null)
			{
				if (model.IsNestedType)
				{
					instance = model2nestedInstance[model];
				}

				if (instance != null) value = prop.GetValue(instance, null);
			}

			String propName = null;

			if (model.IsNestedType)
			{
				propName = String.Format("{0}.{1}", model.Type.Name, prop.Name);
			}
			else
			{
				propName = prop.Name;
			}
			
			if (prop.PropertyType == typeof(DateTime))
			{
				stringBuilder.Append( LabelFor( propName + "day", prop.Name + ": &nbsp;" ) );
			}
			else
			{
				stringBuilder.Append( LabelFor( propName, prop.Name + ": &nbsp;" ) );
			}

			RenderAppropriateControl(model, prop.PropertyType, 
				propName, prop, value, false, false, null, 0);

			return stringBuilder.ToString();
		}

		public String CreateControl(ActiveRecordModel model, BelongsToModel belongsToModel, object instance)
		{
			stringBuilder.Length = 0;

			PropertyInfo prop = belongsToModel.Property;

			ActiveRecordModel otherModel = ActiveRecordModel.GetModel(belongsToModel.BelongsToAtt.Type);

			PrimaryKeyModel keyModel = ObtainPKProperty(otherModel);

			if (otherModel == null || keyModel == null)
			{
				return "Model not found or PK not found";
			}

			object[] items = CommonOperationUtils.FindAll( otherModel.Type );

			String propName = String.Format( "{0}.{1}", prop.Name, keyModel.Property.Name );
			
			object value = null;
				
			if (instance != null)
			{
				if (model.IsNestedType)
				{
					instance = model2nestedInstance[model];
				}

				if (instance != null) value = prop.GetValue( instance, null );
			}

			stringBuilder.Append( LabelFor( propName, prop.Name + ": &nbsp;" ) );

			stringBuilder.Append( Select( propName ) );

			if (!belongsToModel.BelongsToAtt.NotNull)
			{
				stringBuilder.Append( CreateOption("Empty", 0) );
			}

			stringBuilder.Append( CreateOptionsFromArray( items, null, keyModel.Property.Name, value ) );

			stringBuilder.Append( EndSelect() );

			return stringBuilder.ToString();
		}

		public String CreateControl(ActiveRecordModel model, HasManyModel hasManyModel, object instance)
		{
			stringBuilder.Length = 0;

			PropertyInfo prop = hasManyModel.Property;

			ActiveRecordModel otherModel = ActiveRecordModel.GetModel(hasManyModel.HasManyAtt.MapType);

			PrimaryKeyModel keyModel = ObtainPKProperty(otherModel);

			if (otherModel == null || keyModel == null)
			{
				return "Model not found or PK not found";
			}

			object container = InitializeRelationPropertyIfNull(instance, prop);
	
			object value = null;
	
			if (container != null)
			{
				value = CreateArrayFromExistingIds(keyModel, container as ICollection);
			}
	
			object[] items = CommonOperationUtils.FindAll( otherModel.Type, hasManyModel.HasManyAtt.Where );

			String propName = String.Format( "{0}.{1}", prop.Name, keyModel.Property.Name );
			
			stringBuilder.Append( LabelFor( propName, prop.Name + ": &nbsp;" ) );
			stringBuilder.Append( "<br/>" );
			stringBuilder.Append( Select( propName, new DictHelper().CreateDict("size=6", "multiple") ) );
			stringBuilder.Append( CreateOptionsFromArray( items, null, keyModel.Property.Name, value ) );
			stringBuilder.Append( EndSelect() );

			return stringBuilder.ToString();
		}

		public String CreateControl(ActiveRecordModel model, HasAndBelongsToManyModel hasAndBelongsModel, object instance)
		{
			stringBuilder.Length = 0;

			PropertyInfo prop = hasAndBelongsModel.Property;

			ActiveRecordModel otherModel = ActiveRecordModel.GetModel(hasAndBelongsModel.HasManyAtt.MapType);

			PrimaryKeyModel keyModel = ObtainPKProperty(otherModel);

			if (otherModel == null || keyModel == null)
			{
				return "Model not found or PK not found";
			}

			object container = InitializeRelationPropertyIfNull(instance, prop);
	
			object value = null;
	
			if (container != null)
			{
				value = CreateArrayFromExistingIds(keyModel, container as ICollection);
			}
	
			object[] items = CommonOperationUtils.FindAll( otherModel.Type, hasAndBelongsModel.HasManyAtt.Where );

			String propName = String.Format( "{0}.{1}", prop.Name, keyModel.Property.Name );
			
			stringBuilder.Append( LabelFor( propName, prop.Name + ": &nbsp;" ) );
			stringBuilder.Append( "<br/>" );
			stringBuilder.Append( Select( propName, new DictHelper().CreateDict("size=6", "multiple") ) );
			stringBuilder.Append( CreateOptionsFromArray( items, null, keyModel.Property.Name, value ) );
			stringBuilder.Append( EndSelect() );

			return stringBuilder.ToString();
		}

		#endregion

		private void RenderAppropriateControl(ActiveRecordModel model, 
			Type propType, string propName, PropertyInfo property, 
			object value, bool unique, bool notNull, String columnType, int length)
		{
			IDictionary htmlAttributes = new Hashtable();

			htmlAttributes["validators"] = PopulateCustomValidators( 
				notNull ? "blank" : "bok", propType, property, model.Validators );

			if (propType == typeof(String))
			{
				if (String.Compare("stringclob", columnType, true) == 0)
				{
					stringBuilder.AppendFormat( TextArea( propName, 30, 3, (String) value ) );
				}
				else if (length != 0)
				{
					stringBuilder.AppendFormat( InputText( propName, (String) value, length, length, htmlAttributes ) );
				}
				else
				{
					stringBuilder.AppendFormat( InputText( propName, (String) value, htmlAttributes ) );
				}
			}
			else if (propType == typeof(Int16) || propType == typeof(Int32) || propType == typeof(Int64))
			{
				stringBuilder.AppendFormat( InputText( propName, value.ToString(), 10, 4, htmlAttributes ) );
			}
			else if (propType == typeof(Single) || propType == typeof(Double))
			{
				stringBuilder.AppendFormat( InputText( propName, value.ToString(), htmlAttributes) );
			}
			else if (propType == typeof(DateTime))
			{
				stringBuilder.AppendFormat( DateTime( propName, (DateTime) value, htmlAttributes ) );
			}
			else if (propType == typeof(bool))
			{
				// stringBuilder.AppendFormat( InputCheckbox(propName, "true", (bool) value) );
				stringBuilder.Append( Select(propName) );
				stringBuilder.Append( CreateOptionsFromPrimitiveArray(new bool[] { true, false }, value.ToString() ) );
				stringBuilder.Append( EndSelect() );
			}
			else if (propType == typeof(Enum))
			{
				// TODO: Support flags as well

				String[] names = System.Enum.GetNames(propType);

				IList options = new ArrayList();

				foreach(String name in names)
				{
					options.Add( String.Format("{0} {1}\r\n", 
						InputRadio(propName, name), LabelFor(name, name) ) );
				}
				
				stringBuilder.AppendFormat( BuildUnorderedList(options) );
			}
		}

		private String PopulateCustomValidators(string initialValidation, Type type, PropertyInfo property, IList validators)
		{
			ArrayList list = new ArrayList();

			if (property != null)
			{
				foreach(IValidator validator in validators)
				{
					if (validator.Property == property)
					{
						if (validator is NullCheckValidator && initialValidation.Equals("bok"))
						{
							initialValidation = "blank";
						}
						else if (validator is EmailValidator)
						{
							list.Add("email|0");
						}
						else if (validator is ConfirmationValidator)
						{
							// TODO: Change ConfirmationValidator to expose the confirmation target
							// Validate equality with a 2nd field.
							// list.Add("equalto");
						}
					}
				}
			}

			if (type == typeof(Int32))
			{
				list.Add("number|0|" + Int32.MinValue.ToString() + "|" + Int32.MaxValue.ToString());
			}
			else if (type == typeof(Int16))
			{
				list.Add("number|0|" + Int16.MinValue.ToString() + "|" + Int16.MaxValue.ToString());
			}
			else if (type == typeof(Int64))
			{
				list.Add("number|0|" + Int64.MinValue.ToString() + "|" + Int64.MaxValue.ToString());
			}

			if (list.Count == 0)
			{
				return initialValidation.Equals("bok") ? "" : initialValidation;
			}
			else
			{
				if (initialValidation.Equals("bok"))
				{
					list.Add(initialValidation);
				}
				else
				{
					list.Insert(0, initialValidation);
				}
				return String.Join("|", (String[]) list.ToArray( typeof(String) ) );
			}
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

		private static Array CreateArrayFromExistingIds( PrimaryKeyModel keyModel, ICollection container )
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
	}
}
