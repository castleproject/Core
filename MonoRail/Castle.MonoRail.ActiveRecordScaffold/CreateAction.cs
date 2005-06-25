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
	using System.Reflection;
	using System.Collections;
	using System.Collections.Specialized;

	using Castle.MonoRail.Framework;

	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework.Internal;


	public class CreateAction : NewAction
	{
		private ArrayList errors = new ArrayList();

		public CreateAction(Type modelType) : base(modelType)
		{
		}

		public override void Execute(Controller controller)
		{
			ActiveRecordModel model = GetARModel();

			instance = Activator.CreateInstance( model.Type );

			RecursiveSetData( instance, model, controller.Context.Params );

			try
			{
				SaveInstance(instance, controller);
			}
			catch(Exception ex)
			{
				errors.Add( "Could not save " + model.Type.Name + ". " + ex.Message );
			}

			if (errors.Count != 0)
			{
				controller.Context.Flash["errors"] = errors;
				base.Execute(controller);
			}
			else
			{
				String name = model.Type.Name;
				String viewName = String.Format(@"{0}\create{1}", controller.Name, name);

				if (controller.HasTemplate(viewName))
				{
					controller.PropertyBag.Add( "armodel", model );
					controller.PropertyBag.Add( "instance", instance );
					controller.RenderView(controller.Name, "create" + name);
				}
				else
				{
					GenerateSuccessMessage(name, model, instance, controller);
				}
			}
		}

		private void GenerateSuccessMessage(string name, ActiveRecordModel model, object instance, Controller controller)
		{
			controller.DirectRender( "Successfully created!" );
		}

		private void SaveInstance(object instance, Controller controller)
		{
			if (instance is ActiveRecordValidationBase)
			{
				ActiveRecordValidationBase instanceBase = instance as ActiveRecordValidationBase;

				if (!instanceBase.IsValid())
				{
					errors.AddRange(instanceBase.ValidationErrorMessages);
				}
				else
				{
					instanceBase.Save();
				}
			}
			else
			{
				ActiveRecordBase instanceBase = instance as ActiveRecordBase;

				instanceBase.Save();
			}
		}

		private void RecursiveSetData(object instance, ActiveRecordModel model, NameValueCollection valueCollection)
		{
			Type type = model.Type;

			if ( type.BaseType != typeof(object) && 
				 type.BaseType != typeof(ActiveRecordBase) && 
				 type.BaseType != typeof(ActiveRecordValidationBase) )
			{
				RecursiveSetData( instance, ActiveRecordBase._GetModel( type.BaseType ), valueCollection );
			}

			foreach( PropertyModel prop in model.Properties )
			{
				object value = GetConvertedValue( prop.Property.Name, prop.Property.PropertyType, valueCollection );

				if (value == null) continue;

				prop.Property.SetValue( instance, value, null );
			}

			foreach( PropertyInfo prop in model.NotMappedProperties )
			{
				object value = GetConvertedValue( prop.Name, prop.PropertyType, valueCollection );

				if (value == null) continue;

				prop.SetValue( instance, value, null );
			}

			foreach( NestedModel nested in model.Components )
			{
				object nestedInstace = nested.Property.GetGetMethod().Invoke( instance, null );

				if (nestedInstace == null)
				{
					nestedInstace = Activator.CreateInstance( nested.Property.PropertyType );
					nested.Property.SetValue( instance, nestedInstace, null );
				}

				RecursiveSetData( nestedInstace, nested.Model, valueCollection );
			}
		}

		private object GetConvertedValue(String name, Type type, NameValueCollection collection)
		{
			if (type == typeof(DateTime))
			{
				String day = collection[name + "day"];
				String month = collection[name + "month"];
				String year = collection[name + "year"];

				try
				{
					return new DateTime( Convert.ToInt32(year), Convert.ToInt32(month), Convert.ToInt32(day) );
				}
				catch(Exception ex)
				{
					errors.Add( "Invalid date." );
					return null;
				}
			}
			else
			{
				String value = collection[name];

				if (value == null) return null;

				if (type == typeof(String))
				{
					return value;
				}
				else if (type == typeof(Int16))
				{
					return Convert.ToInt16( value );
				}
				else if (type == typeof(Int32))
				{
					return Convert.ToInt32( value );
				} 
				else if (type == typeof(Int64))
				{
					return Convert.ToInt64( value );
				}
				else if (type == typeof(Single))
				{
					return Convert.ToSingle( value );
				}
				else if (type == typeof(Double))
				{
					return Convert.ToDouble( value );
				}
				else 
				{
					throw new ArgumentException("Type " + type.FullName + " is not handled yet");
				}
			}
		}
	}
}
