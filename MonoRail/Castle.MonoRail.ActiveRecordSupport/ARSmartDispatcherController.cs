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

namespace Castle.MonoRail.ActiveRecordSupport
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Reflection;

	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework;
	using Castle.ActiveRecord.Framework.Internal;
	
	using Castle.MonoRail.Framework;


	/// <summary>
	/// Extends the <see cref="SmartDispatcherController"/> 
	/// with ActiveRecord specific functionality
	/// </summary>
	public class ARSmartDispatcherController : SmartDispatcherController
	{
		private ARDataBinder arBinder;

		public ARSmartDispatcherController() : base(new ARDataBinder())
		{
		}

		protected override void Initialize()
		{
			arBinder = (ARDataBinder) Binder;
		}

		/// <summary>
		/// Check if any method argument is associated with an ARDataBinderAttribute
		/// that has the Validate property set to true,
		/// and perform automatic validation for it (as long it inherit from ActiveRecordValidationBase)
		/// </summary>
		protected override object[] BuildMethodArguments(ParameterInfo[] parameters, IRequest request)
		{
			object[] args = base.BuildMethodArguments(parameters, request);

			for(int i = 0; i < args.Length; i++)
			{
				ParameterInfo param = parameters[i];

				object[] bindAttributes = param.GetCustomAttributes(typeof(ARDataBindAttribute), false);

				if (bindAttributes.Length > 0)
				{
					ARDataBindAttribute dba = bindAttributes[0] as ARDataBindAttribute;

					if (dba.AutoPersist)
					{
						PersistInstances(args[i]);
					}
					else if (dba.Validate)
					{
						ValidateInstances(args[i], param);
					}
				}
			}

			return args;
		}

		protected override bool BindComplexParameter(ParameterInfo param, IRequest request, object[] args, int argIndex)
		{
			object[] arBinderAttributes = param.GetCustomAttributes(typeof(ARDataBindAttribute), false);

			if (arBinderAttributes.Length != 0)
			{
				ARDataBindAttribute attr = (ARDataBindAttribute) arBinderAttributes[0];
				
				args[argIndex] = CustomBindObject(attr.From, param.ParameterType, 
					attr.Prefix, attr.NestedLevel, attr.Exclude, attr.Allow, attr.AutoLoad);
				
				return true;
			}

			object[] fetchAttributes = param.GetCustomAttributes(typeof(ARFetchAttribute), false);
			
			if (fetchAttributes.Length != 0)
			{
				ARFetchAttribute attr = (ARFetchAttribute) fetchAttributes[0];
				args[argIndex] = FetchActiveRecord(param, attr, request);
				return true;
			}

			return base.BindComplexParameter(param, request, args, argIndex);
		}

		protected object CustomBindObject(ParamStore from, Type paramType, String prefix, int nestedLevel, 
			String excludedProperties, String allowedProperties, bool autoLoad)
		{
			NameValueCollection webParams = ResolveParamsSource(from);

			ArrayList errorList = new ArrayList();

			object instance = arBinder.BindObject(paramType, prefix, webParams, Context.Request.Files, 
				errorList, nestedLevel, excludedProperties, allowedProperties, autoLoad);

			boundInstances[instance] = errorList;

			return instance;
		}

		protected override String GetRequestParameterName(ParameterInfo param)
		{
			object[] fetchAttributes = param.GetCustomAttributes(typeof(ARFetchAttribute), false);
			
			if (fetchAttributes.Length == 0)
				return base.GetRequestParameterName(param);

			ARFetchAttribute attr = (ARFetchAttribute) fetchAttributes[0];
			
			return attr.RequestParameterName != null
				? attr.RequestParameterName
				: base.GetRequestParameterName(param);
		}

		private object FetchActiveRecord(ParameterInfo param, ARFetchAttribute attr, IRequest request)
		{
			Type type = param.ParameterType;
			
			bool isArray = type.IsArray;

			if (isArray) type = type.GetElementType();

			ActiveRecordModel model = ActiveRecordModel.GetModel(type);

			if (model == null)
			{
				throw new RailsException(String.Format("'{0}' is not an ActiveRecord " + 
					"class. It could not be bound to an [ARFetch] attribute.", type.Name));
			}

			if (model.Ids.Count != 1)
			{
				throw new RailsException("ARFetch only supports single-attribute primary keys");
			}

			String webParamName = attr.RequestParameterName != null ? attr.RequestParameterName : param.Name;

			if (!isArray)
			{
				return LoadActiveRecord(type, request.Params[webParamName], attr, model);
			}

			object[] pks = request.Params.GetValues(webParamName);

			Array objs = Array.CreateInstance(type, pks.Length);

			for(int i = 0; i < objs.Length; i++)
			{
				objs.SetValue(LoadActiveRecord(type, pks[i], attr, model), i);
			}

			return objs;
		}

		private object LoadActiveRecord(Type type, object pk, ARFetchAttribute attr, ActiveRecordModel model)
		{
			object obj = null;

			if (pk != null && !String.Empty.Equals(pk))
			{
				PrimaryKeyModel pkModel = (PrimaryKeyModel) model.Ids[0];

				Type pkType = pkModel.Property.PropertyType;

				if (pk.GetType() != pkType)
					pk = Convert.ChangeType(pk, pkType);

				obj = SupportingUtils.FindByPK(type, pk, attr.Required);
			}

			if (obj == null && attr.Create)
				obj = Activator.CreateInstance(type);

			return obj;
		}

		private void PersistInstances(object instances)
		{
			Type instanceType = instances.GetType();
			ActiveRecordBase[] records = null;

			if (instanceType.IsArray)
			{
				records = instances as ActiveRecordBase[];
			}
			else if (typeof(ActiveRecordBase).IsAssignableFrom(instanceType))
			{
				records = new ActiveRecordBase[] {(ActiveRecordBase) instances};
			}

			if (records != null)
			{
				foreach(ActiveRecordBase record in records)
				{
					record.Save();
				}
			}
		}

		private void ValidateInstances(object instances, ParameterInfo param)
		{
			Type instanceType = instances.GetType();
			ActiveRecordValidationBase[] records = null;

			if (instanceType.IsArray)
			{
				records = instances as ActiveRecordValidationBase[];
			}
			else if (typeof(ActiveRecordValidationBase).IsAssignableFrom(instanceType))
			{
				records = new ActiveRecordValidationBase[] {(ActiveRecordValidationBase) instances};
			}

			if (records != null)
			{
				foreach(ActiveRecordValidationBase record in records)
				{
					if (!record.IsValid())
					{
						throw new RailsException("ARSmartDispatchController: Error validating {0} {1}\n{2}",
						                         param.ParameterType.FullName, param.Name, string.Join("\n", record.ValidationErrorMessages));
					}
				}
			}
		}
	}
}