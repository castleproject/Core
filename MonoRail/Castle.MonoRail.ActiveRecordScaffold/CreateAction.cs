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
		private DataBinder binder;
		private IRailsEngineContext context;

		public CreateAction(Type modelType) : base(modelType)
		{
		}

		protected override string ComputeTemplateName(Controller controller)
		{
			if (controller.Context.Flash["errors"] != null)
			{
				return base.ComputeTemplateName(controller);
			}
			else
			{
				return String.Format(@"{0}\create{1}", controller.Name, Model.Type.Name);
			}
		}

		protected override void PerformActionProcess(Controller controller)
		{
			context = controller.Context;

			binder = new DataBinder(controller.Context);
			
			instance = binder.BindObject( Model.Type );

			try
			{
				SaveInstance(instance, controller);
			}
			catch(Exception ex)
			{
				errors.Add( "Could not save " + Model.Type.Name + ". " + ex.Message );
			}

			if (errors.Count != 0)
			{
				controller.Context.Flash["errors"] = errors;
			}

			controller.PropertyBag["armodel"] = Model;
			controller.PropertyBag["instance"] = instance;
		}

		protected override void RenderStandardHtml(Controller controller)
		{
			if (controller.Context.Flash["errors"] != null)
			{
				base.RenderStandardHtml(controller);
			}
			else
			{
				controller.Redirect( controller.Name, "list" + Model.Type.Name );
			}
		}

		protected void SaveInstance(object instance, Controller controller)
		{
			if (instance is ActiveRecordValidationBase)
			{
				ActiveRecordValidationBase instanceBase = instance as ActiveRecordValidationBase;

				if (!instanceBase.IsValid())
				{
					errors.AddRange(instanceBase.ValidationErrorMessages);
					prop2Validation = instanceBase.PropertiesValidationErrorMessage;
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

//		private void RecursiveSetData(object instance, ActiveRecordModel model, NameValueCollection valueCollection)
//		{
//			Type type = model.Type;
//
//			if ( type.BaseType != typeof(object) && 
//				 type.BaseType != typeof(ActiveRecordBase) && 
//				 type.BaseType != typeof(ActiveRecordValidationBase) )
//			{
//				RecursiveSetData( instance, ActiveRecordBase._GetModel( type.BaseType ), valueCollection );
//			}
//
//			foreach( PropertyModel prop in model.Properties )
//			{
//				object value = DataBinder.Convert( 
//					prop.Property.PropertyType, valueCollection[prop.Property.Name], prop.Property.Name, null, context );
//
//				if (value == null) continue;
//
//				prop.Property.SetValue( instance, value, null );
//			}
//
//			foreach( PropertyInfo prop in model.NotMappedProperties )
//			{
//				object value = DataBinder.Convert( 
//					prop.PropertyType, valueCollection[prop.Name], prop.Name, null, context );
//
//				if (value == null) continue;
//
//				prop.SetValue( instance, value, null );
//			}
//
//			foreach( NestedModel nested in model.Components )
//			{
//				object nestedInstace = nested.Property.GetGetMethod().Invoke( instance, null );
//
//				if (nestedInstace == null)
//				{
//					nestedInstace = Activator.CreateInstance( nested.Property.PropertyType );
//					nested.Property.SetValue( instance, nestedInstace, null );
//				}
//
//				RecursiveSetData( nestedInstace, nested.Model, valueCollection );
//			}
//		}

//		private object GetConvertedValue(String name, Type type, NameValueCollection collection)
//		{
//			if (type == typeof(DateTime))
//			{
//				String day = collection[name + "day"];
//				String month = collection[name + "month"];
//				String year = collection[name + "year"];
//
//				try
//				{
//					return new DateTime( Convert.ToInt32(year), Convert.ToInt32(month), Convert.ToInt32(day) );
//				}
//				catch(Exception)
//				{
//					errors.Add( "Invalid date." );
//					return null;
//				}
//			}
//			else
//			{
//				return DataBinder.Convert(type, )
//
//				String value = collection[name];
//
//				if (value == null) return null;
//
//				if (type == typeof(String))
//				{
//					return value;
//				}
//				else if (type == typeof(Int16))
//				{
//					return Convert.ToInt16( value );
//				}
//				else if (type == typeof(Int32))
//				{
//					return Convert.ToInt32( value );
//				} 
//				else if (type == typeof(Int64))
//				{
//					return Convert.ToInt64( value );
//				}
//				else if (type == typeof(Single))
//				{
//					return Convert.ToSingle( value );
//				}
//				else if (type == typeof(Double))
//				{
//					return Convert.ToDouble( value );
//				}
//				else 
//				{
//					throw new ArgumentException("Type " + type.FullName + " is not handled yet");
//				}
//			}
//		}
	}
}
