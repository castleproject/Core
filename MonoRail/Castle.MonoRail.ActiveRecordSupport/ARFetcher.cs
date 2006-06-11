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
	using System.Reflection;

	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework.Internal;
	using Castle.MonoRail.Framework;

	/// <summary>
	/// Class responsible on loading records for parameters marked with the <see cref="ARFetchAttribute" />.
	/// </summary>
	public class ARFetcher
	{
		public object FetchActiveRecord(ParameterInfo param, ARFetchAttribute attr, IRequest request)
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

			if (pks == null)
			{
				pks = new object[0];
			}

			Array objs = Array.CreateInstance(type, pks.Length);

			for(int i = 0; i < objs.Length; i++)
			{
				objs.SetValue(LoadActiveRecord(type, pks[i], attr, model), i);
			}

			return objs;
		}

		private object LoadActiveRecord(Type type, object pk, ARFetchAttribute attr, ActiveRecordModel model)
		{
			object instance = null;

			if (pk != null && !String.Empty.Equals(pk))
			{
				PrimaryKeyModel pkModel = (PrimaryKeyModel) model.Ids[0];

				Type pkType = pkModel.Property.PropertyType;

				if (pk.GetType() != pkType)
					pk = Convert.ChangeType(pk, pkType);

				instance = ActiveRecordMediator.FindByPrimaryKey(type, pk, attr.Required);
			}

			if (instance == null && attr.Create)
			{
				instance = Activator.CreateInstance(type);
			}

			return instance;
		}
	}
}