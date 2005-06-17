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

namespace Castle.ActiveRecord.Generator.Components
{
	using System;
	using System.Collections;

	using Castle.ActiveRecord.Generator.Components.Database;


	public class PlainFieldInferenceService : IPlainFieldInferenceService
	{
		private INamingService _namingService;
		private ITypeInferenceService _typeInfService;

		public PlainFieldInferenceService(INamingService namingService, ITypeInferenceService typeInfService)
		{
			_namingService = namingService;
			_typeInfService = typeInfService;
		}

		#region IPlainFieldInferenceService Members

		public ActiveRecordPropertyDescriptor[] InferProperties(TableDefinition tableDef)
		{
			ArrayList list = new ArrayList();

			foreach(ColumnDefinition col in tableDef.Columns)
			{
				if (col.ForeignKey) continue;

				String propertyName = _namingService.CreatePropertyName(col.Name);
				Type   propertyType = _typeInfService.ConvertOleType(col.Type);
				String colType = col.Type.ToString();
				String colName = col.Name;
		
				ActiveRecordFieldDescriptor field = null;

				if (col.PrimaryKey)
				{
					field = new ActiveRecordPrimaryKeyDescriptor(
						colName, colType, propertyName, propertyType, "Native");
				}
				else
				{
					field = new ActiveRecordFieldDescriptor(
						colName, colType, propertyName, propertyType, col.Nullable);
				}

				list.Add(field);
			}

			return (ActiveRecordFieldDescriptor[]) list.ToArray( typeof(ActiveRecordFieldDescriptor) );
		}

		#endregion
	}
}
