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
	using System.Collections.Specialized;
	
	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework;
	using Castle.ActiveRecord.Framework.Internal;	

	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// Extends DataBinder class with some ActiveRecord specific functionallity
	/// for example by specifying an "autoload" attribute to your form params
	/// this class will automatically load the database record before biding
	/// any properties values.
	/// </summary>
	public class ARDataBinder : DataBinder
	{
		protected internal static readonly String AutoLoadAttribute = DataBinder.MetadataIdentifier + "autoload";
		
		public ARDataBinder() : base()
		{			
		}

		protected override object CreateInstance( Type instanceType, string paramPrefix, 
			NameValueCollection paramsList )
		{
			object instance = null;
			
			if( paramsList[ paramPrefix + AutoLoadAttribute ] == Yes )
			{
				if( instanceType.IsArray )
				{
					throw new RailsException("ARDataBinder autoload does not support arrays");
				}
				
				if( !typeof(ActiveRecordBase).IsAssignableFrom( instanceType ) )
				{
					throw new RailsException("ARDataBinder autoload only supports classes that inherit from ActiveRecordBase");
				}
					
				ActiveRecordModel model = ActiveRecordBase.GetModel(instanceType);				
				// NOTE: as of right now we only support one PK
				if( model.Ids.Count == 1 )
				{
					PrimaryKeyModel pkModel = model.Ids[0] as PrimaryKeyModel;
					
					string propName = pkModel.Property.Name;
					string paramListPk = ( paramPrefix == String.Empty ) ? propName : paramPrefix + "." + propName;
					string propValue = paramsList.Get( paramListPk );
					
					if( propValue != null )
					{
						object id = ConvertUtils.Convert(pkModel.Property.PropertyType, propValue, propName, null, null );
						instance = SupportingUtils.FindByPK(instanceType, id);
					}
					else
					{
						throw new RailsException( "ARDataBinder autoload failed as element {0} doesn't have a primary key {1} value", paramPrefix, propName );
					}				
				}
			}
			else
			{
				instance = base.CreateInstance(instanceType, paramPrefix, paramsList);
			}

			return instance;
		}
	}
}
