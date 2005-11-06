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

	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework;

	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Internal;


	public abstract class CommonOperationUtils
	{
		internal static object[] FindAll( Type type )
		{
			return FindAll( type, null );
		}

		internal static object[] FindAll( Type type, String customWhere )
		{
			MethodInfo findAll = type.GetMethod( "FindAll", 
				BindingFlags.Static|BindingFlags.Public, null, new Type[0], null );

			object[] items = null;

			if (findAll != null)
			{
				items = (object[]) findAll.Invoke( null, null );
			}
			else
			{
				IList list = SupportingUtils.FindAll( type );

				items = new object[list.Count];

				list.CopyTo(items, 0);
			}

			return items;
		}

		internal static void SaveInstance(object instance, 
			Controller controller, ArrayList errors, IDictionary prop2Validation)
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

		internal static object ReadPkFromParams(Controller controller, PropertyInfo keyProperty)
		{
			String id = controller.Context.Params["id"];
	
			if (id == null)
			{
				throw new ScaffoldException("Can't edit without the proper id");
			}
	
			return ConvertUtils.Convert(keyProperty.PropertyType, 
				id, "id", null, controller.Params );
		}
	}
}
