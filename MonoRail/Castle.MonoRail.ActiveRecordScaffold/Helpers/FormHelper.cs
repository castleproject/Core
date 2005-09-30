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


	public class FormHelper : HtmlHelper
	{
		private StringBuilder stringBuilder = new StringBuilder(1024);

		public ICollection GetModelHierarchy(ActiveRecordModel model)
		{
			ArrayList list = new ArrayList();

			ActiveRecordModel hierarchy = model;

			while(hierarchy != null)
			{
				list.Add(hierarchy);

				hierarchy = ActiveRecordBase._GetModel( model.Type.BaseType );
			}

			hierarchy = model;

			while(hierarchy != null)
			{
				foreach(NestedModel nested in hierarchy.Components)
				{
					list.Add( nested.Model );
				}

				hierarchy = ActiveRecordBase._GetModel( model.Type.BaseType );
			}

			return list;
		}

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

		public String CreateControl(ActiveRecordModel model, FieldModel fieldModel, object instance)
		{
			stringBuilder.Length = 0;

			FieldInfo fieldInfo = fieldModel.Field;

			object value = null;
				
			if (instance != null)
			{
				value = fieldInfo.GetValue(instance);
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

			stringBuilder.Append( LabelFor( fieldInfo.Name, propName + ": &nbsp;" ) );
			
			FieldAttribute propAtt = fieldModel.FieldAtt;

			RenderAppropriateControl(model, fieldInfo.FieldType, propName, value, 
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
				value = prop.GetValue(instance, null);
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
			
			stringBuilder.Append( LabelFor( prop.Name, propName + ": &nbsp;" ) );

			PropertyAttribute propAtt = propertyModel.PropertyAtt;
			
			RenderAppropriateControl(model, prop.PropertyType, propName, value, 
				propAtt.Unique, propAtt.NotNull, propAtt.ColumnType, propAtt.Length);

			return stringBuilder.ToString();
		}

		private void RenderAppropriateControl(ActiveRecordModel model, Type propType, 
			string propName, object value, bool unique, bool notNull, String columnType, int length)
		{
			IDictionary htmlAttributes = new Hashtable();

			htmlAttributes["validators"] = notNull ? "blank" : "bok";

//			foreach(IValidator validator in model.Validators)
//			{
//				if (validator.Property == propertyModel.Property)
//				{
//					if (validator is NullCheckValidator)
//					{
//						htmlAttributes["validators"] = "blank";
//					}
//				}
//			}

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
				stringBuilder.AppendFormat( InputText( propName, value.ToString(), 10, 4 ) );
			}
			else if (propType == typeof(Single) || propType == typeof(Double))
			{
				stringBuilder.AppendFormat( InputText( propName, value.ToString() ) );
			}
			else if (propType == typeof(DateTime))
			{
				stringBuilder.AppendFormat( DateTime( propName, (DateTime) value ) );
			}
			else if (propType == typeof(bool))
			{
				// TODO: checkbox
			}
			else if (propType == typeof(Enum))
			{
				// TODO: select
			}
		}
	}
}
