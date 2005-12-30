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
	using Castle.MonoRail.Framework.Internal;
	
	using Iesi.Collections;

	/// <summary>
	/// Extends DataBinder class with some ActiveRecord specific functionallity
	/// for example by specifying an "autoload" attribute to your form params
	/// this class will automatically load the database record before binding
	/// any properties values.
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class ARDataBinder : DataBinder
	{
		protected internal static readonly object[] EmptyArg = new object[0];
		protected internal static readonly String AutoLoadAttribute = DataBinder.MetadataIdentifier + "autoload";
		
		private bool autoLoad;

		public ARDataBinder() : base()
		{
		}

		public object BindObject(Type instanceType, String paramPrefix, 
			NameValueCollection paramList, IDictionary files, IList errorList, int nestedLevel, 
			String excludedProperties, String allowProperties, bool autoLoad)
		{
			this.autoLoad = true;

			return BindObject(instanceType, paramPrefix, paramList, files, errorList, nestedLevel, 
				excludedProperties, allowProperties);
		}

		protected override object CreateInstance(Type instanceType, String paramPrefix, NameValueCollection paramsList)
		{
			object instance = null;

			bool shouldLoad = autoLoad || paramsList[paramPrefix + AutoLoadAttribute] == Yes;

			if (shouldLoad && paramsList[paramPrefix + AutoLoadAttribute] == No)
			{
				shouldLoad = false;
			}

			if (shouldLoad)
			{
				if (instanceType.IsArray)
				{
					throw new RailsException("ARDataBinder autoload does not support arrays");
				}

				if (!typeof(ActiveRecordBase).IsAssignableFrom(instanceType))
				{
					throw new RailsException("ARDataBinder autoload only supports classes that inherit from ActiveRecordBase");
				}

				ActiveRecordModel model = ActiveRecordModel.GetModel(instanceType);

				// NOTE: as of right now we only support one PK
				if (model.Ids.Count == 1)
				{
					PrimaryKeyModel pkModel = model.Ids[0] as PrimaryKeyModel;

					string propName = pkModel.Property.Name;
					string paramListPk = (paramPrefix == String.Empty) ? propName : paramPrefix + "." + propName;
					string propValue = paramsList.Get(paramListPk);

					if (propValue != null)
					{
						object id = ConvertUtils.Convert(pkModel.Property.PropertyType, propValue, propName, null, null);
						instance = SupportingUtils.FindByPK(instanceType, id);
					}
					else
					{
						throw new RailsException("ARDataBinder autoload failed as element {0} doesn't have a primary key {1} value", paramPrefix, propName);
					}
				}
			}
			else
			{
				instance = base.CreateInstance(instanceType, paramPrefix, paramsList);
			}

			return instance;
		}

		protected override void AfterBinding(object instance, String paramPrefix, DataBindContext context)
		{
			// Defensive programming
			if (instance == null) return;

			ActiveRecordModel model = ActiveRecordModel.GetModel(instance.GetType());

			if (model == null) return;

			SaveManyMappings(instance, model, context);
		}

		protected void SaveManyMappings(object instance, ActiveRecordModel model, DataBindContext context)
		{
			foreach(HasManyModel hasManyModel in model.HasMany)
			{
				if (hasManyModel.HasManyAtt.Inverse) continue;
				if (hasManyModel.HasManyAtt.RelationType != RelationType.Bag &&
					hasManyModel.HasManyAtt.RelationType != RelationType.Set) continue;

				ActiveRecordModel otherModel = ActiveRecordModel.GetModel(hasManyModel.HasManyAtt.MapType);

				PrimaryKeyModel keyModel = ARCommonUtils.ObtainPKProperty(otherModel);

				if (otherModel == null || keyModel == null)
				{
					continue; // Impossible to save
				}

				CreateMappedInstances(instance, hasManyModel.Property, keyModel, otherModel, context);
			}

			foreach(HasAndBelongsToManyModel hasManyModel in model.HasAndBelongsToMany)
			{
				if (hasManyModel.HasManyAtt.Inverse) continue;
				if (hasManyModel.HasManyAtt.RelationType != RelationType.Bag &&
					hasManyModel.HasManyAtt.RelationType != RelationType.Set) continue;

				ActiveRecordModel otherModel = ActiveRecordModel.GetModel(hasManyModel.HasManyAtt.MapType);

				PrimaryKeyModel keyModel = ARCommonUtils.ObtainPKProperty(otherModel);

				if (otherModel == null || keyModel == null)
				{
					continue; // Impossible to save
				}

				CreateMappedInstances(instance, hasManyModel.Property, keyModel, otherModel, context);
			}
		}

		private void CreateMappedInstances(object instance, PropertyInfo prop,
		                                   PrimaryKeyModel keyModel, ActiveRecordModel otherModel, DataBindContext context)
		{
			object container = InitializeRelationPropertyIfNull(instance, prop);

			// TODO: Support any kind of key

			String paramName = String.Format("{0}.{1}", prop.Name, keyModel.Property.Name);

			String[] values = context.ParamList.GetValues(paramName);

			int[] ids = (int[]) ConvertUtils.Convert(typeof(int[]), values, paramName, null, context.ParamList);

			if (ids != null)
			{
				foreach(int id in ids)
				{
					object item = Activator.CreateInstance(otherModel.Type);

					keyModel.Property.SetValue(item, id, EmptyArg);

					AddToContainer(container, item);
				}
			}
		}

		private static object InitializeRelationPropertyIfNull(object instance, PropertyInfo property)
		{
			object container = property.GetValue(instance, EmptyArg);

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

				property.SetValue(instance, container, EmptyArg);
			}

			return container;
		}

		private void AddToContainer(object container, object item)
		{
			if (container is IList)
			{
				(container as IList).Add(item);
			}
			else if (container is ISet)
			{
				(container as ISet).Add(item);
			}
		}
	}
}